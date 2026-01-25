using FinnhubSdk.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FinnhubSdk.Tests.Integration;

/// <summary>
/// Test fixture for integration tests with rate limit management
/// </summary>
public class FinnhubTestFixture : IDisposable
{
    private const int DelayBetweenTestsMs = 2000; // 2 seconds between tests for free tier (60 req/min)
    private static readonly SemaphoreSlim RateLimitSemaphore = new(1, 1);
    private readonly ServiceProvider _serviceProvider;

    public FinnhubTestFixture()
    {
        var services = new ServiceCollection();

        // API key from environment variable (GitHub secret or local testing)
        var apiKey = Environment.GetEnvironmentVariable("FINNHUB_API_KEY")
            ?? throw new InvalidOperationException(
                "FINNHUB_API_KEY environment variable must be set for integration tests. " +
                "Get a free API key from https://finnhub.io/register");

        services.AddFinnhub(options =>
        {
            options.ApiKey = apiKey;
            options.MaxRetries = 1; // Reduce retries to avoid quota exhaustion
            options.EnableLogging = true;
        });

        services.AddLogging();
        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Executes an action with rate limit protection
    /// Ensures 2-second delay between API calls to respect free tier limits
    /// </summary>
    public async Task<T> ExecuteWithRateLimitAsync<T>(Func<Task<T>> action)
    {
        await RateLimitSemaphore.WaitAsync();
        try
        {
            var result = await action();
            await Task.Delay(DelayBetweenTestsMs); // Wait before next test
            return result;
        }
        finally
        {
            RateLimitSemaphore.Release();
        }
    }

    /// <summary>
    /// Gets a service from the DI container
    /// </summary>
    public T GetService<T>() where T : notnull
        => _serviceProvider.GetRequiredService<T>();

    public void Dispose()
    {
        _serviceProvider?.Dispose();
        GC.SuppressFinalize(this);
    }
}
