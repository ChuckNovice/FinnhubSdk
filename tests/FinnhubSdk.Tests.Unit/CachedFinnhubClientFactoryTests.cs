using FinnhubSdk.Clients;
using FinnhubSdk.Configuration;
using FinnhubSdk.Services.Crypto;
using FinnhubSdk.Services.Economic;
using FinnhubSdk.Services.Forex;
using FinnhubSdk.Services.News;
using FinnhubSdk.Services.Stocks;
using FinnhubSdk.WebSocket;
using Microsoft.Extensions.Options;
using Moq;

namespace FinnhubSdk.Tests.Unit;

[TestClass]
public class CachedFinnhubClientFactoryTests
{
    private Mock<IFinnhubClientFactory> _mockFactory = null!;
    private CachedFinnhubClientFactory _cachedFactory = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockFactory = new Mock<IFinnhubClientFactory>();
        _mockFactory
            .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(() => CreateMockClient());

        var options = Options.Create(new CachedFinnhubClientFactoryOptions());
        _cachedFactory = new CachedFinnhubClientFactory(_mockFactory.Object, options);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        if (_cachedFactory != null)
        {
            await _cachedFactory.DisposeAsync();
        }
    }

    private static IFinnhubClient CreateMockClient()
    {
        var mockWebSocket = new Mock<IFinnhubWebSocketClient>();
        mockWebSocket.Setup(w => w.DisposeAsync()).Returns(ValueTask.CompletedTask);

        var mockClient = new Mock<IFinnhubClient>();
        mockClient.Setup(c => c.WebSocket).Returns(mockWebSocket.Object);
        mockClient.Setup(c => c.Stocks).Returns(Mock.Of<IStocksService>());
        mockClient.Setup(c => c.News).Returns(Mock.Of<INewsService>());
        mockClient.Setup(c => c.Forex).Returns(Mock.Of<IForexService>());
        mockClient.Setup(c => c.Crypto).Returns(Mock.Of<ICryptoService>());
        mockClient.Setup(c => c.Economic).Returns(Mock.Of<IEconomicService>());

        return mockClient.Object;
    }

    [TestMethod]
    public void GetOrCreateClient_FirstCall_CreatesClient()
    {
        var client = _cachedFactory.GetOrCreateClient("key1");

        Assert.IsNotNull(client);
        _mockFactory.Verify(f => f.CreateClient("key1"), Times.Once);
    }

    [TestMethod]
    public void GetOrCreateClient_SecondCallSameKey_ReturnsCachedInstance()
    {
        var client1 = _cachedFactory.GetOrCreateClient("key1");
        var client2 = _cachedFactory.GetOrCreateClient("key1");

        Assert.AreSame(client1, client2);
        _mockFactory.Verify(f => f.CreateClient("key1"), Times.Once);
    }

    [TestMethod]
    public void GetOrCreateClient_DifferentKeys_CreatesDifferentClients()
    {
        var client1 = _cachedFactory.GetOrCreateClient("key1");
        var client2 = _cachedFactory.GetOrCreateClient("key2");

        Assert.AreNotSame(client1, client2);
        _mockFactory.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Exactly(2));
    }

    [TestMethod]
    public async Task RemoveAsync_RemovesClientFromCache()
    {
        var client1 = _cachedFactory.GetOrCreateClient("key1");
        await _cachedFactory.RemoveAsync("key1");
        var client2 = _cachedFactory.GetOrCreateClient("key1");

        Assert.AreNotSame(client1, client2);
        _mockFactory.Verify(f => f.CreateClient("key1"), Times.Exactly(2));
    }

    [TestMethod]
    public async Task RemoveAsync_NonExistentKey_DoesNotThrow()
    {
        await _cachedFactory.RemoveAsync("nonexistent");
        // Should complete without exception
    }

    [TestMethod]
    public async Task ClearAsync_RemovesAllClients()
    {
        var client1 = _cachedFactory.GetOrCreateClient("key1");
        var client2 = _cachedFactory.GetOrCreateClient("key2");

        await _cachedFactory.ClearAsync();

        var client1New = _cachedFactory.GetOrCreateClient("key1");
        var client2New = _cachedFactory.GetOrCreateClient("key2");

        Assert.AreNotSame(client1, client1New);
        Assert.AreNotSame(client2, client2New);
    }

    [TestMethod]
    public async Task DisposeAsync_DisposesAllCachedClients()
    {
        var mockWebSocket1 = new Mock<IFinnhubWebSocketClient>();
        mockWebSocket1.Setup(w => w.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var mockClient1 = new Mock<IFinnhubClient>();
        mockClient1.Setup(c => c.WebSocket).Returns(mockWebSocket1.Object);

        var mockWebSocket2 = new Mock<IFinnhubWebSocketClient>();
        mockWebSocket2.Setup(w => w.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var mockClient2 = new Mock<IFinnhubClient>();
        mockClient2.Setup(c => c.WebSocket).Returns(mockWebSocket2.Object);

        _mockFactory.SetupSequence(f => f.CreateClient(It.IsAny<string>()))
            .Returns(mockClient1.Object)
            .Returns(mockClient2.Object);

        _cachedFactory.GetOrCreateClient("key1");
        _cachedFactory.GetOrCreateClient("key2");

        await _cachedFactory.DisposeAsync();

        mockWebSocket1.Verify(w => w.DisposeAsync(), Times.AtLeastOnce);
        mockWebSocket2.Verify(w => w.DisposeAsync(), Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task DisposeAsync_CalledTwice_IsIdempotent()
    {
        _cachedFactory.GetOrCreateClient("key1");

        await _cachedFactory.DisposeAsync();
        await _cachedFactory.DisposeAsync(); // Should not throw
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public async Task GetOrCreateClient_AfterDispose_ThrowsObjectDisposedException()
    {
        await _cachedFactory.DisposeAsync();
        _cachedFactory.GetOrCreateClient("key1");
    }

    [TestMethod]
    public void GetOrCreateClient_ConcurrentCalls_ReturnsSameInstance()
    {
        const int threadCount = 10;
        var clients = new IFinnhubClient[threadCount];
        var barrier = new Barrier(threadCount);

        var tasks = Enumerable.Range(0, threadCount)
            .Select(i => Task.Run(() =>
            {
                barrier.SignalAndWait();
                clients[i] = _cachedFactory.GetOrCreateClient("concurrent-key");
            }))
            .ToArray();

        Task.WaitAll(tasks);

        // All should be the same instance
        Assert.IsTrue(clients.All(c => ReferenceEquals(c, clients[0])));

        // Factory should only be called once
        _mockFactory.Verify(f => f.CreateClient("concurrent-key"), Times.Once);
    }

    [TestMethod]
    public void GetOrCreateClient_ConcurrentCallsDifferentKeys_CreatesSeparateClients()
    {
        const int keyCount = 5;
        var clients = new System.Collections.Concurrent.ConcurrentDictionary<string, IFinnhubClient>();
        var barrier = new Barrier(keyCount * 2);

        var tasks = Enumerable.Range(0, keyCount)
            .SelectMany(i => new[]
            {
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    clients[$"key{i}"] = _cachedFactory.GetOrCreateClient($"key{i}");
                }),
                Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    clients[$"key{i}-dup"] = _cachedFactory.GetOrCreateClient($"key{i}");
                })
            })
            .ToArray();

        Task.WaitAll(tasks);

        // Verify each key got the same instance for duplicate calls
        for (int i = 0; i < keyCount; i++)
        {
            Assert.AreSame(clients[$"key{i}"], clients[$"key{i}-dup"]);
        }
    }
}
