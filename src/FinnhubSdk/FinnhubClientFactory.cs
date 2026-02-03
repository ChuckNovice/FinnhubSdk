using FinnhubSdk.Clients;
using FinnhubSdk.Configuration;
using FinnhubSdk.Services.Crypto;
using FinnhubSdk.Services.Economic;
using FinnhubSdk.Services.Forex;
using FinnhubSdk.Services.News;
using FinnhubSdk.Services.Stocks;
using FinnhubSdk.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinnhubSdk;

/// <summary>
/// Stateless factory for creating Finnhub clients with dynamic API keys.
/// Uses ActivatorUtilities to leverage DI for service creation while allowing per-client options.
/// </summary>
internal sealed class FinnhubClientFactory : IFinnhubClientFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FinnhubClientOptions _sharedOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubClientFactory"/> class
    /// </summary>
    /// <param name="serviceProvider">Service provider for resolving dependencies</param>
    /// <param name="httpClientFactory">HTTP client factory</param>
    /// <param name="sharedOptions">Shared configuration options</param>
    public FinnhubClientFactory(
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory,
        IOptions<FinnhubClientOptions> sharedOptions)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _sharedOptions = sharedOptions?.Value ?? throw new ArgumentNullException(nameof(sharedOptions));
    }

    /// <inheritdoc/>
    public IFinnhubClient CreateClient(string apiKey)
    {
        ArgumentNullException.ThrowIfNull(apiKey);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key cannot be empty or whitespace", nameof(apiKey));
        }

        return CreateClient(o => o.ApiKey = apiKey);
    }

    /// <inheritdoc/>
    public IFinnhubClient CreateClient(Action<FinnhubOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        // Build options: start with shared defaults, then apply user customization
        var options = new FinnhubOptions
        {
            BaseUrl = _sharedOptions.BaseUrl,
            WebSocketUrl = _sharedOptions.WebSocketUrl,
            AuthMethod = _sharedOptions.AuthMethod,
            Timeout = _sharedOptions.Timeout,
            MaxRetries = _sharedOptions.MaxRetries,
            RetryDelay = _sharedOptions.RetryDelay,
            EnableLogging = _sharedOptions.EnableLogging,
            RateLimitStrategy = _sharedOptions.RateLimitStrategy
        };
        configureOptions(options);

        // Validate that API key was set
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new InvalidOperationException("API key must be configured");
        }

        var wrappedOptions = Options.Create(options);

        // Create HTTP client from named client (gets the Polly policies)
        var httpClient = _httpClientFactory.CreateClient("FinnhubFactory");
        ConfigureHttpClient(httpClient, options);

        // Use ActivatorUtilities to create services with DI
        // This resolves ILogger<T> from DI while allowing us to override specific parameters
        var finnhubHttp = ActivatorUtilities.CreateInstance<FinnhubHttpClient>(
            _serviceProvider,
            httpClient,
            wrappedOptions);

        var stocks = ActivatorUtilities.CreateInstance<StocksService>(_serviceProvider, finnhubHttp);
        var news = ActivatorUtilities.CreateInstance<NewsService>(_serviceProvider, finnhubHttp);
        var forex = ActivatorUtilities.CreateInstance<ForexService>(_serviceProvider, finnhubHttp);
        var crypto = ActivatorUtilities.CreateInstance<CryptoService>(_serviceProvider, finnhubHttp);
        var economic = ActivatorUtilities.CreateInstance<EconomicService>(_serviceProvider, finnhubHttp);
        var webSocket = ActivatorUtilities.CreateInstance<FinnhubWebSocketClient>(_serviceProvider, wrappedOptions);

        return ActivatorUtilities.CreateInstance<FinnhubClient>(
            _serviceProvider,
            stocks,
            news,
            forex,
            crypto,
            economic,
            webSocket);
    }

    private static void ConfigureHttpClient(HttpClient httpClient, FinnhubOptions options)
    {
        // Ensure base URL ends with '/' for proper relative URI resolution
        var baseUrl = options.BaseUrl.EndsWith('/') ? options.BaseUrl : options.BaseUrl + "/";
        httpClient.BaseAddress = new Uri(baseUrl);
        httpClient.Timeout = options.Timeout;
    }
}
