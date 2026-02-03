using FinnhubSdk.Configuration;
using FinnhubSdk.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinnhubSdk.Tests.Unit;

[TestClass]
public class FinnhubClientFactoryTests
{
    private ServiceProvider _serviceProvider = null!;
    private IFinnhubClientFactory _factory = null!;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddFinnhubFactory();
        _serviceProvider = services.BuildServiceProvider();
        _factory = _serviceProvider.GetRequiredService<IFinnhubClientFactory>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _serviceProvider?.Dispose();
    }

    [TestMethod]
    public void CreateClient_WithApiKey_ReturnsClient()
    {
        var client = _factory.CreateClient("test-api-key");

        Assert.IsNotNull(client);
        Assert.IsNotNull(client.Stocks);
        Assert.IsNotNull(client.News);
        Assert.IsNotNull(client.Forex);
        Assert.IsNotNull(client.Crypto);
        Assert.IsNotNull(client.Economic);
        Assert.IsNotNull(client.WebSocket);
    }

    [TestMethod]
    public void CreateClient_WithConfigureOptions_AppliesOptions()
    {
        var client = _factory.CreateClient(o =>
        {
            o.ApiKey = "test-key";
            o.Timeout = TimeSpan.FromSeconds(120);
        });

        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void CreateClient_CalledTwice_ReturnsNewInstances()
    {
        var client1 = _factory.CreateClient("key1");
        var client2 = _factory.CreateClient("key1");

        Assert.AreNotSame(client1, client2);
    }

    [TestMethod]
    public void CreateClient_DifferentKeys_ReturnsDifferentInstances()
    {
        var client1 = _factory.CreateClient("key1");
        var client2 = _factory.CreateClient("key2");

        Assert.AreNotSame(client1, client2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreateClient_NullApiKey_ThrowsArgumentNullException()
    {
        _factory.CreateClient((string)null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateClient_EmptyApiKey_ThrowsArgumentException()
    {
        _factory.CreateClient(string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreateClient_NullConfigureOptions_ThrowsArgumentNullException()
    {
        _factory.CreateClient((Action<FinnhubOptions>)null!);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CreateClient_ConfigureOptionsWithoutApiKey_ThrowsInvalidOperationException()
    {
        _factory.CreateClient(o =>
        {
            // Don't set ApiKey
            o.Timeout = TimeSpan.FromSeconds(30);
        });
    }
}
