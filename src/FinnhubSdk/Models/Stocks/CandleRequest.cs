using System.ComponentModel.DataAnnotations;

namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Request parameters for retrieving candle (OHLCV) data
/// </summary>
public sealed class CandleRequest
{
    /// <summary>
    /// Stock symbol (e.g., "AAPL", "MSFT")
    /// </summary>
    [Required]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Candle resolution (time interval)
    /// </summary>
    public CandleResolution Resolution { get; set; } = CandleResolution.Day;

    /// <summary>
    /// Start date/time for the candle data
    /// </summary>
    [Required]
    public DateTime From { get; set; }

    /// <summary>
    /// End date/time for the candle data
    /// </summary>
    [Required]
    public DateTime To { get; set; }
}
