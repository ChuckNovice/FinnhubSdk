namespace FinnhubSdk.WebSocket;

using FinnhubSdk.WebSocket.Models;

/// <summary>
/// WebSocket client for real-time trade streaming from Finnhub
/// </summary>
public interface IFinnhubWebSocketClient : IAsyncDisposable
{
    /// <summary>
    /// Current connection state
    /// </summary>
    ConnectionState State { get; }

    /// <summary>
    /// Gets the set of currently subscribed symbols
    /// </summary>
    IReadOnlySet<string> SubscribedSymbols { get; }

    /// <summary>
    /// Async callback invoked when trade data is received
    /// Use this instead of events to allow awaitable operations
    /// </summary>
    Func<TradeMessage, Task>? OnTradeReceived { get; set; }

    /// <summary>
    /// Async callback invoked when the connection state changes
    /// Use this instead of events to allow awaitable operations
    /// </summary>
    Func<ConnectionState, Task>? OnConnectionStateChanged { get; set; }

    /// <summary>
    /// Async callback invoked when an error occurs
    /// Use this instead of events to allow awaitable operations
    /// </summary>
    Func<Exception, Task>? OnErrorOccurred { get; set; }

    /// <summary>
    /// Connects to the Finnhub WebSocket server
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when connected</returns>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the Finnhub WebSocket server
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when disconnected</returns>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to real-time trades for a symbol
    /// </summary>
    /// <param name="symbol">Symbol to subscribe to (e.g., "AAPL", "BINANCE:BTCUSDT")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when subscribed</returns>
    Task SubscribeAsync(string symbol, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to real-time trades for multiple symbols
    /// </summary>
    /// <param name="symbols">Symbols to subscribe to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when all subscriptions are sent</returns>
    Task SubscribeAsync(IEnumerable<string> symbols, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes from real-time trades for a symbol
    /// </summary>
    /// <param name="symbol">Symbol to unsubscribe from</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when unsubscribed</returns>
    Task UnsubscribeAsync(string symbol, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes from all symbols
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when all unsubscriptions are sent</returns>
    Task UnsubscribeAllAsync(CancellationToken cancellationToken = default);
}
