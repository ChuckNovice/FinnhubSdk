using FinnhubSdk.Models.Stocks;

namespace FinnhubSdk.Services.Stocks;

/// <summary>
/// Service for accessing stock market data from Finnhub API
/// </summary>
public interface IStocksService
{
    /// <summary>
    /// Gets real-time quote data for a stock symbol
    /// </summary>
    /// <param name="symbol">Stock symbol (e.g., "AAPL", "MSFT")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Real-time quote data</returns>
    Task<Quote> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets historical candle (OHLCV) data for a stock symbol
    /// </summary>
    /// <param name="request">Candle request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of candles for the requested time period</returns>
    Task<IReadOnlyList<Candle>> GetCandlesAsync(CandleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets company profile information
    /// </summary>
    /// <param name="symbol">Stock symbol (e.g., "AAPL", "MSFT")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Company profile data</returns>
    Task<CompanyProfile> GetCompanyProfileAsync(string symbol, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for stock symbols matching a query
    /// </summary>
    /// <param name="query">Search query (company name or symbol)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching stock symbols</returns>
    Task<IReadOnlyList<StockSymbol>> SearchSymbolsAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets basic financial metrics for a company including P/E ratios, margins, and valuation metrics
    /// </summary>
    /// <param name="symbol">Stock symbol (e.g., "AAPL", "MSFT")</param>
    /// <param name="metric">Metric type: "all", "price", or "valuation" (default: "all")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Basic financial metrics for the company</returns>
    Task<BasicFinancials> GetBasicFinancialsAsync(string symbol, string metric = "all", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about company executives and board members
    /// </summary>
    /// <param name="symbol">Stock symbol (e.g., "AAPL", "MSFT")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of company executives with their titles, compensation, and other details</returns>
    Task<IReadOnlyList<CompanyExecutive>> GetCompanyExecutivesAsync(string symbol, CancellationToken cancellationToken = default);
}
