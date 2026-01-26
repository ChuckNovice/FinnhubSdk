using FinnhubSdk.Clients;
using FinnhubSdk.Configuration;
using FinnhubSdk.Infrastructure.Handlers;
using FinnhubSdk.Infrastructure.Policies;
using FinnhubSdk.Services.Forex;
using FinnhubSdk.Services.News;
using FinnhubSdk.Services.Stocks;
using FinnhubSdk.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinnhubSdk.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register Finnhub SDK services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Finnhub SDK services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Options configuration action</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddFinnhub(
        this IServiceCollection services,
        Action<FinnhubOptions> configureOptions)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureOptions == null)
        {
            throw new ArgumentNullException(nameof(configureOptions));
        }

        // Configure options with validation
        services.AddOptions<FinnhubOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register options validator
        services.AddSingleton<IValidateOptions<FinnhubOptions>, FinnhubOptionsValidator>();

        // Register HTTP message handlers
        services.AddTransient<FinnhubAuthenticationHandler>();
        services.AddTransient<RateLimitHandler>();

        // Configure HTTP client with handlers and policies
        services.AddHttpClient<FinnhubHttpClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<FinnhubOptions>>().Value;
            // Ensure base URL ends with '/' for proper relative URI resolution
            var baseUrl = options.BaseUrl.EndsWith('/') ? options.BaseUrl : options.BaseUrl + "/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = options.Timeout;
        })
        .AddHttpMessageHandler<FinnhubAuthenticationHandler>()
        .AddHttpMessageHandler<RateLimitHandler>()
        .AddPolicyHandler((serviceProvider, _) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<FinnhubOptions>>().Value;
            var logger = serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<FinnhubHttpClient>>();

            return FinnhubPolicies.GetRetryPolicy(
                options.MaxRetries,
                options.RetryDelay,
                logger);
        })
        .AddPolicyHandler((serviceProvider, _) =>
        {
            var logger = serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<FinnhubHttpClient>>();
            return FinnhubPolicies.GetCircuitBreakerPolicy(logger: logger);
        })
        .AddPolicyHandler((serviceProvider, _) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<FinnhubOptions>>().Value;
            var logger = serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<FinnhubHttpClient>>();

            return FinnhubPolicies.GetTimeoutPolicy(
                options.Timeout,
                logger);
        });

        // Register service interfaces
        services.AddScoped<IStocksService, StocksService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IForexService, ForexService>();

        // Register WebSocket client as singleton (maintains connection across requests)
        services.AddSingleton<IFinnhubWebSocketClient, FinnhubWebSocketClient>();

        // Register main client facade
        services.AddScoped<IFinnhubClient, FinnhubClient>();

        return services;
    }

    /// <summary>
    /// Adds Finnhub SDK services using configuration from IConfiguration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddFinnhub(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        return services.AddFinnhub(options =>
        {
            configuration.GetSection(FinnhubOptions.SectionName).Bind(options);
        });
    }
}
