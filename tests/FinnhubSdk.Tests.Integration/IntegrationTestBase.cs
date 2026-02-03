using FinnhubSdk.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FinnhubSdk.Tests.Integration;

[TestClass]
public abstract class IntegrationTestBase
{
    protected static string ApiKey => Environment.GetEnvironmentVariable("FINNHUB_API_KEY")
        ?? throw new InvalidOperationException("FINNHUB_API_KEY environment variable not set");

    protected IFinnhubClientFactory Factory { get; private set; } = null!;
    protected ICachedFinnhubClientFactory CachedFactory { get; private set; } = null!;
    private ServiceProvider _serviceProvider = null!;

    [TestInitialize]
    public void TestSetup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCachedFinnhubFactory();
        _serviceProvider = services.BuildServiceProvider();
        Factory = _serviceProvider.GetRequiredService<IFinnhubClientFactory>();
        CachedFactory = _serviceProvider.GetRequiredService<ICachedFinnhubClientFactory>();
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        if (_serviceProvider != null)
        {
            await _serviceProvider.DisposeAsync();
        }
    }

    /// <summary>
    /// Call before each API request to respect rate limits (60 calls/min = 1 per second).
    /// </summary>
    protected static async Task RateLimitDelayAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1.5));
    }
}
