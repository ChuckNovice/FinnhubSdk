namespace FinnhubSdk.Services.Forex;

using FinnhubSdk.Models.Stocks;

/// <summary>
/// Service for accessing forex market data from Finnhub API
/// </summary>
public interface IForexService
{
    /// <summary>
    /// Gets historical candle (OHLCV) data for a forex pair
    /// </summary>
    /// <param name="symbol">Forex pair symbol (e.g., "OANDA:EUR_USD")</param>
    /// <param name="resolution">Candle resolution (1, 5, 15, 30, 60, D, W, M)</param>
    /// <param name="from">Start date/time</param>
    /// <param name="to">End date/time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of candles for the requested time period</returns>
    Task<IReadOnlyList<Candle>> GetCandlesAsync(string symbol, CandleResolution resolution, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
