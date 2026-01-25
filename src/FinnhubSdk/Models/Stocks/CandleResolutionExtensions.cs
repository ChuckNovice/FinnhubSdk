namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Extension methods for <see cref="CandleResolution"/>
/// </summary>
internal static class CandleResolutionExtensions
{
    /// <summary>
    /// Converts the enum value to the Finnhub API string representation
    /// </summary>
    /// <param name="resolution">The resolution enum value</param>
    /// <returns>API string representation (1, 5, 15, 30, 60, D, W, M)</returns>
    public static string ToApiString(this CandleResolution resolution)
    {
        return resolution switch
        {
            CandleResolution.OneMinute => "1",
            CandleResolution.FiveMinutes => "5",
            CandleResolution.FifteenMinutes => "15",
            CandleResolution.ThirtyMinutes => "30",
            CandleResolution.SixtyMinutes => "60",
            CandleResolution.Day => "D",
            CandleResolution.Week => "W",
            CandleResolution.Month => "M",
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, "Invalid candle resolution")
        };
    }
}
