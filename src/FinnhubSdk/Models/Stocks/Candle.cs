namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// OHLCV candle data for a specific time interval
/// </summary>
public sealed class Candle
{
    /// <summary>
    /// Open price
    /// </summary>
    public decimal Open { get; set; }

    /// <summary>
    /// High price
    /// </summary>
    public decimal High { get; set; }

    /// <summary>
    /// Low price
    /// </summary>
    public decimal Low { get; set; }

    /// <summary>
    /// Close price
    /// </summary>
    public decimal Close { get; set; }

    /// <summary>
    /// Trading volume
    /// </summary>
    public long Volume { get; set; }

    /// <summary>
    /// Timestamp of the candle
    /// </summary>
    public DateTime Timestamp { get; set; }
}
