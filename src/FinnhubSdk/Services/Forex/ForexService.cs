using FinnhubSdk.Clients;
using FinnhubSdk.Models.Stocks;
using Microsoft.Extensions.Logging;

namespace FinnhubSdk.Services.Forex;

/// <summary>
/// Implementation of <see cref="IForexService"/> for accessing forex market data
/// </summary>
internal sealed class ForexService : IForexService
{
    private readonly FinnhubHttpClient _httpClient;
    private readonly ILogger<ForexService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForexService"/> class
    /// </summary>
    /// <param name="httpClient">HTTP client for API requests</param>
    /// <param name="logger">Logger instance</param>
    public ForexService(FinnhubHttpClient httpClient, ILogger<ForexService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Candle>> GetCandlesAsync(string symbol, CandleResolution resolution, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or whitespace", nameof(symbol));
        }

        if (from >= to)
        {
            throw new ArgumentException("From date must be before To date", nameof(from));
        }

        _logger.LogDebug(
            "Getting forex candles for symbol: {Symbol}, resolution: {Resolution}, from: {From}, to: {To}",
            symbol,
            resolution,
            from,
            to);

        var fromTimestamp = new DateTimeOffset(from).ToUnixTimeSeconds();
        var toTimestamp = new DateTimeOffset(to).ToUnixTimeSeconds();
        var resolutionString = resolution.ToApiString();

        var response = await _httpClient.GetAsync<CandleResponse>(
            $"forex/candle?symbol={Uri.EscapeDataString(symbol)}&resolution={resolutionString}&from={fromTimestamp}&to={toTimestamp}",
            null,
            cancellationToken);

        if (response == null)
        {
            _logger.LogWarning("No forex candle data returned for symbol: {Symbol}", symbol);
            return Array.Empty<Candle>();
        }

        return response.ToCandles();
    }
}
