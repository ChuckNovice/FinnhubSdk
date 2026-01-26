namespace FinnhubSdk.Services.News;

using FinnhubSdk.Models.News;

/// <summary>
/// Service for accessing news data from Finnhub API
/// </summary>
public interface INewsService
{
    /// <summary>
    /// Gets company news articles for a specific symbol within a date range
    /// </summary>
    /// <param name="symbol">Stock symbol (e.g., "AAPL", "MSFT")</param>
    /// <param name="from">Start date for news</param>
    /// <param name="to">End date for news</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of news articles</returns>
    Task<IReadOnlyList<NewsArticle>> GetCompanyNewsAsync(
        string symbol,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets general market news for a specific category
    /// </summary>
    /// <param name="category">News category (e.g., "general", "forex", "crypto", "merger")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of news articles</returns>
    Task<IReadOnlyList<NewsArticle>> GetMarketNewsAsync(
        string category = "general",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets news sentiment analysis for a company
    /// </summary>
    /// <param name="symbol">Stock symbol (e.g., "AAPL", "MSFT")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>News sentiment data</returns>
    Task<NewsSentiment> GetNewsSentimentAsync(
        string symbol,
        CancellationToken cancellationToken = default);
}
