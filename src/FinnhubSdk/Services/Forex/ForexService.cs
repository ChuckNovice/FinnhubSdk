namespace FinnhubSdk.Services.Forex;

using FinnhubSdk.Clients;
using FinnhubSdk.Models.Stocks;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementation of <see cref="IForexService"/> for accessing forex market data
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ForexService"/> class
/// </remarks>
/// <param name="httpClient">HTTP client for API requests</param>
/// <param name="logger">Logger instance</param>
internal sealed class ForexService(FinnhubHttpClient httpClient, ILogger<ForexService> logger) : IForexService
{
    private readonly FinnhubHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<ForexService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
            return [];
        }

        return response.ToCandles();
    }
}
