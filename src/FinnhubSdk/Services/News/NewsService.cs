using FinnhubSdk.Clients;
using FinnhubSdk.Models.News;
using Microsoft.Extensions.Logging;

namespace FinnhubSdk.Services.News;

/// <summary>
/// Implementation of <see cref="INewsService"/> for accessing news data
/// </summary>
internal sealed class NewsService : INewsService
{
    private readonly FinnhubHttpClient _httpClient;
    private readonly ILogger<NewsService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewsService"/> class
    /// </summary>
    /// <param name="httpClient">HTTP client for API requests</param>
    /// <param name="logger">Logger instance</param>
    public NewsService(FinnhubHttpClient httpClient, ILogger<NewsService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<NewsArticle>> GetCompanyNewsAsync(
        string symbol,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        if (from >= to)
        {
            throw new ArgumentException("From date must be before To date");
        }

        _logger.LogDebug(
            "Getting company news for symbol: {Symbol}, from: {From}, to: {To}",
            symbol,
            from,
            to);

        var fromDate = from.ToString("yyyy-MM-dd");
        var toDate = to.ToString("yyyy-MM-dd");

        var articles = await _httpClient.GetAsync<NewsArticle[]>(
            $"company-news?symbol={Uri.EscapeDataString(symbol)}&from={fromDate}&to={toDate}",
            null,
            cancellationToken);

        return articles ?? Array.Empty<NewsArticle>();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<NewsArticle>> GetMarketNewsAsync(
        string category = "general",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category cannot be null or whitespace", nameof(category));
        }

        _logger.LogDebug("Getting market news for category: {Category}", category);

        var articles = await _httpClient.GetAsync<NewsArticle[]>(
            $"news?category={Uri.EscapeDataString(category)}",
            null,
            cancellationToken);

        return articles ?? Array.Empty<NewsArticle>();
    }

    /// <inheritdoc/>
    public async Task<NewsSentiment> GetNewsSentimentAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        _logger.LogDebug("Getting news sentiment for symbol: {Symbol}", symbol);

        var sentiment = await _httpClient.GetAsync<NewsSentiment>(
            $"news-sentiment?symbol={Uri.EscapeDataString(symbol)}",
            null,
            cancellationToken);

        return sentiment ?? throw new InvalidOperationException($"Failed to retrieve news sentiment for symbol: {symbol}");
    }
}
