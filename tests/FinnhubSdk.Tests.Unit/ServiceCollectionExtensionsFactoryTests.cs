using FinnhubSdk.Clients;
using FinnhubSdk.Configuration;
using FinnhubSdk.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinnhubSdk.Tests.Unit;

[TestClass]
public class ServiceCollectionExtensionsFactoryTests
{
    [TestMethod]
    public void AddFinnhubFactory_RegistersIFinnhubClientFactory()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddFinnhubFactory();

        var provider = services.BuildServiceProvider();
        var factory = provider.GetService<IFinnhubClientFactory>();

        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public void AddFinnhubFactory_WithConfigure_AppliesOptions()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddFinnhubFactory(o =>
        {
            o.BaseUrl = "https://custom.api.com";
            o.Timeout = TimeSpan.FromSeconds(120);
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<FinnhubClientOptions>>();

        Assert.AreEqual("https://custom.api.com", options.Value.BaseUrl);
        Assert.AreEqual(TimeSpan.FromSeconds(120), options.Value.Timeout);
    }

    [TestMethod]
    public void AddCachedFinnhubFactory_RegistersBothFactories()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCachedFinnhubFactory();

        var provider = services.BuildServiceProvider();

        Assert.IsNotNull(provider.GetService<IFinnhubClientFactory>());
        Assert.IsNotNull(provider.GetService<ICachedFinnhubClientFactory>());
    }

    [TestMethod]
    public void AddCachedFinnhubFactory_WithConfigure_AppliesCacheOptions()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCachedFinnhubFactory(
            configureCache: o =>
            {
                o.CacheOptions.SizeLimit = 500;
                o.EntryOptions.SlidingExpiration = TimeSpan.FromMinutes(60);
            });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CachedFinnhubClientFactoryOptions>>();

        Assert.AreEqual(500, options.Value.CacheOptions.SizeLimit);
        Assert.AreEqual(TimeSpan.FromMinutes(60), options.Value.EntryOptions.SlidingExpiration);
    }

    [TestMethod]
    public void AddCachedFinnhubFactory_WithClientConfigure_AppliesClientOptions()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCachedFinnhubFactory(
            configureClient: o =>
            {
                o.BaseUrl = "https://custom.finnhub.io";
                o.Timeout = TimeSpan.FromSeconds(90);
            });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<FinnhubClientOptions>>();

        Assert.AreEqual("https://custom.finnhub.io", options.Value.BaseUrl);
        Assert.AreEqual(TimeSpan.FromSeconds(90), options.Value.Timeout);
    }

    [TestMethod]
    public void AddFinnhub_RegistersIFinnhubClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddFinnhub(o => o.ApiKey = "test-key");

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<IFinnhubClient>();

        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void AddFinnhubFactory_CalledMultipleTimes_DoesNotThrow()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddFinnhubFactory();
        services.AddFinnhubFactory(); // Second call should not throw

        var provider = services.BuildServiceProvider();
        var factory = provider.GetService<IFinnhubClientFactory>();

        Assert.IsNotNull(factory);
    }

    [TestMethod]
    public void AddFinnhubFactory_FactoryIsSingleton()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddFinnhubFactory();

        var provider = services.BuildServiceProvider();
        var factory1 = provider.GetRequiredService<IFinnhubClientFactory>();
        var factory2 = provider.GetRequiredService<IFinnhubClientFactory>();

        Assert.AreSame(factory1, factory2);
    }

    [TestMethod]
    public void AddCachedFinnhubFactory_CachedFactoryIsSingleton()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCachedFinnhubFactory();

        var provider = services.BuildServiceProvider();
        var factory1 = provider.GetRequiredService<ICachedFinnhubClientFactory>();
        var factory2 = provider.GetRequiredService<ICachedFinnhubClientFactory>();

        Assert.AreSame(factory1, factory2);
    }
}
