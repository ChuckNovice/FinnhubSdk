using FinnhubSdk.Clients;
using FinnhubSdk.Models.Stocks;
using Microsoft.Extensions.Logging;

namespace FinnhubSdk.Services.Stocks;

/// <summary>
/// Implementation of <see cref="IStocksService"/> for accessing stock market data
/// </summary>
internal sealed class StocksService : IStocksService
{
    private readonly FinnhubHttpClient _httpClient;
    private readonly ILogger<StocksService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StocksService"/> class
    /// </summary>
    /// <param name="httpClient">HTTP client for API requests</param>
    /// <param name="logger">Logger instance</param>
    public StocksService(FinnhubHttpClient httpClient, ILogger<StocksService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<Quote> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        _logger.LogDebug("Getting quote for symbol: {Symbol}", symbol);

        var quote = await _httpClient.GetAsync<Quote>(
            $"quote?symbol={Uri.EscapeDataString(symbol)}",
            null,
            cancellationToken);

        return quote ?? throw new InvalidOperationException($"Failed to retrieve quote for symbol: {symbol}");
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Candle>> GetCandlesAsync(CandleRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(request));
        }

        if (request.From >= request.To)
        {
            throw new ArgumentException("From date must be before To date", nameof(request));
        }

        _logger.LogDebug(
            "Getting candles for symbol: {Symbol}, resolution: {Resolution}, from: {From}, to: {To}",
            request.Symbol,
            request.Resolution,
            request.From,
            request.To);

        var fromTimestamp = new DateTimeOffset(request.From).ToUnixTimeSeconds();
        var toTimestamp = new DateTimeOffset(request.To).ToUnixTimeSeconds();
        var resolution = request.Resolution.ToApiString();

        var response = await _httpClient.GetAsync<CandleResponse>(
            $"stock/candle?symbol={Uri.EscapeDataString(request.Symbol)}&resolution={resolution}&from={fromTimestamp}&to={toTimestamp}",
            null,
            cancellationToken);

        if (response == null)
        {
            _logger.LogWarning("No candle data returned for symbol: {Symbol}", request.Symbol);
            return Array.Empty<Candle>();
        }

        return response.ToCandles();
    }

    /// <inheritdoc/>
    public async Task<CompanyProfile> GetCompanyProfileAsync(string symbol, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        _logger.LogDebug("Getting company profile for symbol: {Symbol}", symbol);

        var profile = await _httpClient.GetAsync<CompanyProfile>(
            $"stock/profile2?symbol={Uri.EscapeDataString(symbol)}",
            null,
            cancellationToken);

        return profile ?? throw new InvalidOperationException($"Failed to retrieve company profile for symbol: {symbol}");
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<StockSymbol>> SearchSymbolsAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query cannot be null or whitespace", nameof(query));
        }

        _logger.LogDebug("Searching symbols with query: {Query}", query);

        var response = await _httpClient.GetAsync<SymbolSearchResponse>(
            $"search?q={Uri.EscapeDataString(query)}",
            null,
            cancellationToken);

        if (response == null || response.Result.Length == 0)
        {
            _logger.LogDebug("No symbols found for query: {Query}", query);
            return Array.Empty<StockSymbol>();
        }

        return response.Result;
    }

    /// <inheritdoc/>
    public async Task<BasicFinancials> GetBasicFinancialsAsync(string symbol, string metric = "all", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        if (string.IsNullOrWhiteSpace(metric))
        {
            throw new ArgumentException("Metric cannot be null or whitespace", nameof(metric));
        }

        _logger.LogDebug("Getting basic financials for symbol: {Symbol}, metric: {Metric}", symbol, metric);

        var financials = await _httpClient.GetAsync<BasicFinancials>(
            $"stock/metric?symbol={Uri.EscapeDataString(symbol)}&metric={Uri.EscapeDataString(metric)}",
            null,
            cancellationToken);

        return financials ?? throw new InvalidOperationException($"Failed to retrieve basic financials for symbol: {symbol}");
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CompanyExecutive>> GetCompanyExecutivesAsync(string symbol, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        _logger.LogDebug("Getting company executives for symbol: {Symbol}", symbol);

        var response = await _httpClient.GetAsync<CompanyExecutiveResponse>(
            $"stock/executive?symbol={Uri.EscapeDataString(symbol)}",
            null,
            cancellationToken);

        if (response == null || response.Executive.Length == 0)
        {
            _logger.LogDebug("No executives found for symbol: {Symbol}", symbol);
            return Array.Empty<CompanyExecutive>();
        }

        return response.Executive;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<EarningsCalendarEntry>> GetEarningsCalendarAsync(DateTime from, DateTime to, string? symbol = null, CancellationToken cancellationToken = default)
    {
        if (from >= to)
        {
            throw new ArgumentException("From date must be before To date", nameof(from));
        }

        _logger.LogDebug("Getting earnings calendar from: {From}, to: {To}, symbol: {Symbol}", from, to, symbol);

        var fromDate = from.ToString("yyyy-MM-dd");
        var toDate = to.ToString("yyyy-MM-dd");
        var url = $"calendar/earnings?from={fromDate}&to={toDate}";

        if (!string.IsNullOrWhiteSpace(symbol))
        {
            url += $"&symbol={Uri.EscapeDataString(symbol)}";
        }

        var response = await _httpClient.GetAsync<EarningsCalendarResponse>(
            url,
            null,
            cancellationToken);

        if (response == null || response.EarningsCalendar.Length == 0)
        {
            _logger.LogDebug("No earnings calendar entries found for the specified criteria");
            return Array.Empty<EarningsCalendarEntry>();
        }

        return response.EarningsCalendar;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<InsiderSentimentEntry>> GetInsiderSentimentAsync(string symbol, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        if (from >= to)
        {
            throw new ArgumentException("From date must be before To date", nameof(from));
        }

        _logger.LogDebug("Getting insider sentiment for symbol: {Symbol}, from: {From}, to: {To}", symbol, from, to);

        var fromDate = from.ToString("yyyy-MM-dd");
        var toDate = to.ToString("yyyy-MM-dd");

        var response = await _httpClient.GetAsync<InsiderSentimentResponse>(
            $"stock/insider-sentiment?symbol={Uri.EscapeDataString(symbol)}&from={fromDate}&to={toDate}",
            null,
            cancellationToken);

        if (response == null || response.Data.Length == 0)
        {
            _logger.LogDebug("No insider sentiment data found for symbol: {Symbol}", symbol);
            return Array.Empty<InsiderSentimentEntry>();
        }

        return response.Data;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GetCompanyPeersAsync(string symbol, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        _logger.LogDebug("Getting company peers for symbol: {Symbol}", symbol);

        var response = await _httpClient.GetAsync<string[]>(
            $"stock/peers?symbol={Uri.EscapeDataString(symbol)}",
            null,
            cancellationToken);

        if (response == null || response.Length == 0)
        {
            _logger.LogDebug("No peers found for symbol: {Symbol}", symbol);
            return Array.Empty<string>();
        }

        return response;
    }
}
