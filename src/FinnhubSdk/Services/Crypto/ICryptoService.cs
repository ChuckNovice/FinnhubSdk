using FinnhubSdk.Models.Stocks;

namespace FinnhubSdk.Services.Crypto;

/// <summary>
/// Service for accessing cryptocurrency market data from Finnhub API
/// </summary>
public interface ICryptoService
{
    /// <summary>
    /// Gets historical candle (OHLCV) data for a cryptocurrency
    /// </summary>
    /// <param name="symbol">Crypto symbol (e.g., "BINANCE:BTCUSDT")</param>
    /// <param name="resolution">Candle resolution (1, 5, 15, 30, 60, D, W, M)</param>
    /// <param name="from">Start date/time</param>
    /// <param name="to">End date/time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of candles for the requested time period</returns>
    Task<IReadOnlyList<Candle>> GetCandlesAsync(string symbol, CandleResolution resolution, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
