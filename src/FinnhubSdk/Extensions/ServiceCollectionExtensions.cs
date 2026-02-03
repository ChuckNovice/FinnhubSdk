using FinnhubSdk.Clients;
using FinnhubSdk.Configuration;
using FinnhubSdk.Infrastructure.Handlers;
using FinnhubSdk.Infrastructure.Policies;
using FinnhubSdk.Services.Crypto;
using FinnhubSdk.Services.Economic;
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
    private const string FactoryHttpClientName = "FinnhubFactory";

    /// <summary>
    /// Adds Finnhub SDK services to the dependency injection container.
    /// Use this method when you have a single API key configured at startup.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Options configuration action</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddFinnhub(
        this IServiceCollection services,
        Action<FinnhubOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

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
        services.AddScoped<ICryptoService, CryptoService>();
        services.AddScoped<IEconomicService, EconomicService>();

        // Register WebSocket client as singleton (maintains connection across requests)
        services.AddSingleton<IFinnhubWebSocketClient, FinnhubWebSocketClient>();

        // Register main client facade
        services.AddScoped<IFinnhubClient, FinnhubClient>();

        return services;
    }

    /// <summary>
    /// Adds Finnhub SDK services using configuration from IConfiguration.
    /// Use this method when you have a single API key configured at startup.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddFinnhub(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        return services.AddFinnhub(options =>
        {
            configuration.GetSection(FinnhubOptions.SectionName).Bind(options);
        });
    }

    /// <summary>
    /// Registers <see cref="IFinnhubClientFactory"/> for creating clients with dynamic API keys.
    /// Use this method when you need to create multiple clients with different API keys at runtime.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configure">Optional action to configure shared client options (URLs, timeouts, etc.)</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddFinnhubFactory(
        this IServiceCollection services,
        Action<FinnhubClientOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Configure shared options (no API key required)
        services.AddOptions<FinnhubClientOptions>()
            .Configure(o => configure?.Invoke(o));

        // Register HTTP message handlers (transient for factory usage)
        services.AddTransient<FinnhubAuthenticationHandler>();
        services.AddTransient<RateLimitHandler>();

        // Configure named HttpClient for factory-created clients
        services.AddHttpClient(FactoryHttpClientName)
            .AddHttpMessageHandler<FinnhubAuthenticationHandler>()
            .AddHttpMessageHandler<RateLimitHandler>()
            .AddPolicyHandler((serviceProvider, _) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<FinnhubClientOptions>>().Value;
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
                var options = serviceProvider.GetRequiredService<IOptions<FinnhubClientOptions>>().Value;
                var logger = serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<FinnhubHttpClient>>();

                return FinnhubPolicies.GetTimeoutPolicy(
                    options.Timeout,
                    logger);
            });

        // Register the stateless factory
        services.AddSingleton<IFinnhubClientFactory, FinnhubClientFactory>();

        return services;
    }

    /// <summary>
    /// Registers <see cref="ICachedFinnhubClientFactory"/> for creating and caching clients with dynamic API keys.
    /// Automatically includes <see cref="IFinnhubClientFactory"/> registration.
    /// Use this method for multi-tenant scenarios where you want automatic caching and disposal of clients.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureCache">Optional action to configure cache options (expiration, size limits, etc.)</param>
    /// <param name="configureClient">Optional action to configure shared client options (URLs, timeouts, etc.)</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddCachedFinnhubFactory(
        this IServiceCollection services,
        Action<CachedFinnhubClientFactoryOptions>? configureCache = null,
        Action<FinnhubClientOptions>? configureClient = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register the stateless factory first
        services.AddFinnhubFactory(configureClient);

        // Configure cache options
        services.AddOptions<CachedFinnhubClientFactoryOptions>()
            .Configure(o => configureCache?.Invoke(o));

        // Register the cached factory
        services.AddSingleton<ICachedFinnhubClientFactory, CachedFinnhubClientFactory>();

        return services;
    }
}
