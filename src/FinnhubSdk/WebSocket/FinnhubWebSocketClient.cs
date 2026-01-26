namespace FinnhubSdk.WebSocket;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using FinnhubSdk.Configuration;
using FinnhubSdk.Exceptions;
using FinnhubSdk.WebSocket.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// WebSocket client for real-time trade streaming from Finnhub with auto-reconnect
/// </summary>
internal sealed class FinnhubWebSocketClient : IFinnhubWebSocketClient
{
    private readonly FinnhubOptions _options;
    private readonly ILogger<FinnhubWebSocketClient> _logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly HashSet<string> _subscribedSymbols = new(StringComparer.OrdinalIgnoreCase);
    private readonly Lock _symbolsLock = new();
    private readonly JsonSerializerOptions _jsonOptions;

    private ClientWebSocket? _webSocket;
    private CancellationTokenSource? _receiveCts;
    private Task? _receiveTask;
    private ConnectionState _state = ConnectionState.Disconnected;
    private int _reconnectAttempts;
    private bool _disposed;

    private const int MaxReconnectAttempts = 5;
    private static readonly TimeSpan[] ReconnectDelays =
    [
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(4),
        TimeSpan.FromSeconds(8),
        TimeSpan.FromSeconds(16),
        TimeSpan.FromSeconds(32)
    ];

    /// <inheritdoc/>
    public ConnectionState State => _state;

    /// <inheritdoc/>
    public IReadOnlySet<string> SubscribedSymbols
    {
        get
        {
            lock (_symbolsLock)
            {
                return new HashSet<string>(_subscribedSymbols, StringComparer.OrdinalIgnoreCase);
            }
        }
    }

    /// <inheritdoc/>
    public Func<TradeMessage, Task>? OnTradeReceived { get; set; }

    /// <inheritdoc/>
    public Func<ConnectionState, Task>? OnConnectionStateChanged { get; set; }

    /// <inheritdoc/>
    public Func<Exception, Task>? OnErrorOccurred { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubWebSocketClient"/> class
    /// </summary>
    /// <param name="options">Finnhub configuration options</param>
    /// <param name="logger">Logger instance</param>
    public FinnhubWebSocketClient(
        IOptions<FinnhubOptions> options,
        ILogger<FinnhubWebSocketClient> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc/>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_state is ConnectionState.Connected or ConnectionState.Connecting)
            {
                _logger.LogDebug("WebSocket already connected or connecting, skipping");
                return;
            }

            await ConnectInternalAsync(cancellationToken);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            await DisconnectInternalAsync(cancellationToken);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task SubscribeAsync(string symbol, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        if (_state != ConnectionState.Connected)
        {
            throw new FinnhubWebSocketException("WebSocket is not connected. Call ConnectAsync first.");
        }

        var message = WebSocketMessage.Subscribe(symbol);
        await SendMessageAsync(message, cancellationToken);

        lock (_symbolsLock)
        {
            _subscribedSymbols.Add(symbol);
        }

        _logger.LogDebug("Subscribed to symbol: {Symbol}", symbol);
    }

    /// <inheritdoc/>
    public async Task SubscribeAsync(IEnumerable<string> symbols, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        foreach (var symbol in symbols)
        {
            await SubscribeAsync(symbol, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(string symbol, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        if (_state != ConnectionState.Connected)
        {
            // Just remove from tracked symbols if not connected
            lock (_symbolsLock)
            {
                _subscribedSymbols.Remove(symbol);
            }
            return;
        }

        var message = WebSocketMessage.Unsubscribe(symbol);
        await SendMessageAsync(message, cancellationToken);

        lock (_symbolsLock)
        {
            _subscribedSymbols.Remove(symbol);
        }

        _logger.LogDebug("Unsubscribed from symbol: {Symbol}", symbol);
    }

    /// <inheritdoc/>
    public async Task UnsubscribeAllAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        string[] symbolsToUnsubscribe;
        lock (_symbolsLock)
        {
            symbolsToUnsubscribe = [.. _subscribedSymbols];
        }

        foreach (var symbol in symbolsToUnsubscribe)
        {
            await UnsubscribeAsync(symbol, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            await DisconnectInternalAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during WebSocket disposal");
        }

        _connectionLock.Dispose();
        _sendLock.Dispose();
    }

    private async Task ConnectInternalAsync(CancellationToken cancellationToken)
    {
        await SetStateAsync(ConnectionState.Connecting);

        try
        {
            _webSocket?.Dispose();
            _webSocket = new ClientWebSocket();

            var uri = new Uri($"{_options.WebSocketUrl}?token={_options.ApiKey}");
            _logger.LogDebug("Connecting to WebSocket: {Uri}", _options.WebSocketUrl);

            await _webSocket.ConnectAsync(uri, cancellationToken);

            _receiveCts = new CancellationTokenSource();
            _receiveTask = ReceiveLoopAsync(_receiveCts.Token);

            await SetStateAsync(ConnectionState.Connected);
            _reconnectAttempts = 0;

            _logger.LogInformation("WebSocket connected successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to WebSocket");
            await SetStateAsync(ConnectionState.Disconnected);
            throw new FinnhubWebSocketException("Failed to connect to Finnhub WebSocket", ex);
        }
    }

    private async Task DisconnectInternalAsync(CancellationToken cancellationToken)
    {
        if (_state == ConnectionState.Disconnected)
        {
            return;
        }

        _logger.LogDebug("Disconnecting WebSocket");

        // Cancel the receive loop
        _receiveCts?.Cancel();

        if (_receiveTask != null)
        {
            try
            {
                await _receiveTask.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Receive task did not complete within timeout");
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        // Close the WebSocket
        if (_webSocket?.State == WebSocketState.Open)
        {
            try
            {
                await _webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Client disconnecting",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error closing WebSocket gracefully");
            }
        }

        _webSocket?.Dispose();
        _webSocket = null;
        _receiveCts?.Dispose();
        _receiveCts = null;

        await SetStateAsync(ConnectionState.Disconnected);
        _logger.LogInformation("WebSocket disconnected");
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];

        try
        {
            while (!cancellationToken.IsCancellationRequested &&
                   _webSocket?.State == WebSocketState.Open)
            {
                using var messageStream = new MemoryStream();

                WebSocketReceiveResult result;
                do
                {
                    result = await _webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogWarning("WebSocket server initiated close");
                        await HandleDisconnectionAsync(cancellationToken);
                        return;
                    }

                    messageStream.Write(buffer, 0, result.Count);
                }
                while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageText = Encoding.UTF8.GetString(messageStream.ToArray());
                    await ProcessMessageAsync(messageText);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during disconnect
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, "WebSocket error in receive loop");
            await InvokeErrorCallbackAsync(ex);
            await HandleDisconnectionAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in receive loop");
            await InvokeErrorCallbackAsync(ex);
            await HandleDisconnectionAsync(CancellationToken.None);
        }
    }

    private async Task ProcessMessageAsync(string messageText)
    {
        try
        {
            // Check for ping message
            if (messageText.Contains("\"type\":\"ping\""))
            {
                _logger.LogTrace("Received ping from server");
                return;
            }

            // Parse trade message
            var tradeMessage = JsonSerializer.Deserialize<TradeMessage>(messageText, _jsonOptions);

            if (tradeMessage?.Type == "trade" && tradeMessage.Data.Length > 0)
            {
                _logger.LogTrace("Received {Count} trades", tradeMessage.Data.Length);
                await InvokeTradeCallbackAsync(tradeMessage);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse WebSocket message: {Message}", messageText);
        }
    }

    private async Task HandleDisconnectionAsync(CancellationToken cancellationToken)
    {
        if (_disposed || _state == ConnectionState.Failed)
        {
            return;
        }

        await SetStateAsync(ConnectionState.Reconnecting);

        while (_reconnectAttempts < MaxReconnectAttempts && !_disposed)
        {
            var delay = ReconnectDelays[Math.Min(_reconnectAttempts, ReconnectDelays.Length - 1)];
            _reconnectAttempts++;

            _logger.LogInformation(
                "Attempting to reconnect (attempt {Attempt}/{MaxAttempts}) in {Delay}s",
                _reconnectAttempts,
                MaxReconnectAttempts,
                delay.TotalSeconds);

            try
            {
                await Task.Delay(delay, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                await _connectionLock.WaitAsync(cancellationToken);
                try
                {
                    await ConnectInternalAsync(cancellationToken);
                    await ResubscribeAllAsync(cancellationToken);
                    _logger.LogInformation("Reconnection successful");
                    return;
                }
                finally
                {
                    _connectionLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Reconnection attempt {Attempt} failed", _reconnectAttempts);
            }
        }

        _logger.LogError("Max reconnection attempts reached, giving up");
        await SetStateAsync(ConnectionState.Failed);
        await InvokeErrorCallbackAsync(
            new FinnhubWebSocketException($"Failed to reconnect after {MaxReconnectAttempts} attempts"));
    }

    private async Task ResubscribeAllAsync(CancellationToken cancellationToken)
    {
        string[] symbolsToResubscribe;
        lock (_symbolsLock)
        {
            symbolsToResubscribe = [.. _subscribedSymbols];
        }

        if (symbolsToResubscribe.Length == 0)
        {
            return;
        }

        _logger.LogDebug("Resubscribing to {Count} symbols", symbolsToResubscribe.Length);

        foreach (var symbol in symbolsToResubscribe)
        {
            try
            {
                var message = WebSocketMessage.Subscribe(symbol);
                await SendMessageAsync(message, cancellationToken);
                _logger.LogTrace("Resubscribed to {Symbol}", symbol);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resubscribe to {Symbol}", symbol);
            }
        }
    }

    private async Task SendMessageAsync(WebSocketMessage message, CancellationToken cancellationToken)
    {
        if (_webSocket?.State != WebSocketState.Open)
        {
            throw new FinnhubWebSocketException("WebSocket is not connected");
        }

        var json = JsonSerializer.Serialize(message, _jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);

        await _sendLock.WaitAsync(cancellationToken);
        try
        {
            await _webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                cancellationToken);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    private async Task SetStateAsync(ConnectionState newState)
    {
        var oldState = _state;
        _state = newState;

        if (oldState != newState)
        {
            _logger.LogDebug("Connection state changed: {OldState} -> {NewState}", oldState, newState);
            await InvokeStateChangedCallbackAsync(newState);
        }
    }

    private async Task InvokeTradeCallbackAsync(TradeMessage tradeMessage)
    {
        var callback = OnTradeReceived;
        if (callback != null)
        {
            try
            {
                await callback(tradeMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in trade callback");
            }
        }
    }

    private async Task InvokeStateChangedCallbackAsync(ConnectionState state)
    {
        var callback = OnConnectionStateChanged;
        if (callback != null)
        {
            try
            {
                await callback(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in state changed callback");
            }
        }
    }

    private async Task InvokeErrorCallbackAsync(Exception exception)
    {
        var callback = OnErrorOccurred;
        if (callback != null)
        {
            try
            {
                await callback(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in error callback");
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FinnhubWebSocketClient));
        }
    }
}
