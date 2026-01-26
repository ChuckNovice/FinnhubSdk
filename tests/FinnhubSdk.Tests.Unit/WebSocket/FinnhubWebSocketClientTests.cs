namespace FinnhubSdk.Tests.Unit.WebSocket;

using FinnhubSdk.Configuration;
using FinnhubSdk.Exceptions;
using FinnhubSdk.WebSocket;
using FinnhubSdk.WebSocket.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public class FinnhubWebSocketClientTests
{
    private Mock<IOptions<FinnhubOptions>> _mockOptions = null!;
    private Mock<ILogger<FinnhubWebSocketClient>> _mockLogger = null!;
    private FinnhubOptions _options = null!;

    [TestInitialize]
    public void Setup()
    {
        _options = new FinnhubOptions
        {
            ApiKey = "test-api-key",
            WebSocketUrl = "wss://ws.finnhub.io"
        };

        _mockOptions = new Mock<IOptions<FinnhubOptions>>();
        _mockOptions.Setup(x => x.Value).Returns(_options);

        _mockLogger = new Mock<ILogger<FinnhubWebSocketClient>>();
    }

    [TestMethod]
    public async Task Constructor_ValidParameters_CreatesInstance()
    {
        // Act
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);

        // Assert
        Assert.IsNotNull(client);
        Assert.AreEqual(ConnectionState.Disconnected, client.State);
        Assert.AreEqual(0, client.SubscribedSymbols.Count);
    }

    [TestMethod]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
                // Act
                _ = new FinnhubWebSocketClient(null!, _mockLogger.Object));
    }

    [TestMethod]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
                // Act
                _ = new FinnhubWebSocketClient(_mockOptions.Object, null!));
    }

    [TestMethod]
    public async Task State_InitialState_IsDisconnected()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);

        // Assert
        Assert.AreEqual(ConnectionState.Disconnected, client.State);
    }

    [TestMethod]
    public async Task SubscribedSymbols_InitialState_IsEmpty()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);

        // Assert
        Assert.AreEqual(0, client.SubscribedSymbols.Count);
    }

    [TestMethod]
    public async Task OnTradeReceived_CanSetCallback()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);
        static Task callback(TradeMessage _) => Task.CompletedTask;

        // Act
        client.OnTradeReceived = callback;

        // Assert
        Assert.AreEqual(callback, client.OnTradeReceived);
    }

    [TestMethod]
    public async Task OnConnectionStateChanged_CanSetCallback()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);
        static Task callback(ConnectionState _) => Task.CompletedTask;

        // Act
        client.OnConnectionStateChanged = callback;

        // Assert
        Assert.AreEqual(callback, client.OnConnectionStateChanged);
    }

    [TestMethod]
    public async Task OnErrorOccurred_CanSetCallback()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);
        static Task callback(Exception _) => Task.CompletedTask;

        // Act
        client.OnErrorOccurred = callback;

        // Assert
        Assert.AreEqual(callback, client.OnErrorOccurred);
    }

    [TestMethod]
    public async Task SubscribeAsync_NotConnected_ThrowsException()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);
        await Assert.ThrowsExceptionAsync<FinnhubWebSocketException>(async () =>

                // Act - Should throw because not connected
                await client.SubscribeAsync("AAPL"));
    }

    [TestMethod]
    public async Task SubscribeAsync_NullSymbol_ThrowsArgumentException()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>

                // Act - Cast to string to avoid ambiguity
                await client.SubscribeAsync((string)null!));
    }

    [TestMethod]
    public async Task SubscribeAsync_EmptySymbol_ThrowsArgumentException()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>

                // Act
                await client.SubscribeAsync(string.Empty));
    }

    [TestMethod]
    public async Task UnsubscribeAsync_NotConnected_RemovesFromTrackedSymbols()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);

        // Act - Should not throw, just removes from tracked symbols
        await client.UnsubscribeAsync("AAPL");

        // Assert
        Assert.AreEqual(0, client.SubscribedSymbols.Count);
    }

    [TestMethod]
    public async Task UnsubscribeAsync_NullSymbol_ThrowsArgumentException()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>

                // Act
                await client.UnsubscribeAsync(null!));
    }

    [TestMethod]
    public async Task UnsubscribeAllAsync_NotConnected_DoesNotThrow()
    {
        // Arrange
        await using var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);

        // Act - Should not throw
        await client.UnsubscribeAllAsync();

        // Assert
        Assert.AreEqual(0, client.SubscribedSymbols.Count);
    }

    [TestMethod]
    public async Task DisposeAsync_DisposedClient_CanBeCalledMultipleTimes()
    {
        // Arrange
        var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);

        // Act - Should not throw
        await client.DisposeAsync();
        await client.DisposeAsync(); // Second call should be safe

        // Assert - No exception thrown
    }

    [TestMethod]
    public async Task ConnectAsync_DisposedClient_ThrowsObjectDisposedException()
    {
        // Arrange
        var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);
        await client.DisposeAsync();
        await Assert.ThrowsExceptionAsync<ObjectDisposedException>(async () =>

                // Act
                await client.ConnectAsync());
    }

    [TestMethod]
    public async Task SubscribeAsync_DisposedClient_ThrowsObjectDisposedException()
    {
        // Arrange
        var client = new FinnhubWebSocketClient(_mockOptions.Object, _mockLogger.Object);
        await client.DisposeAsync();
        await Assert.ThrowsExceptionAsync<ObjectDisposedException>(async () =>

                // Act
                await client.SubscribeAsync("AAPL"));
    }
}
