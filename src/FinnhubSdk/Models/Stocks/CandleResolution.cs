namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Resolution for candle data (time interval)
/// </summary>
public enum CandleResolution
{
    /// <summary>
    /// 1 minute
    /// </summary>
    OneMinute,

    /// <summary>
    /// 5 minutes
    /// </summary>
    FiveMinutes,

    /// <summary>
    /// 15 minutes
    /// </summary>
    FifteenMinutes,

    /// <summary>
    /// 30 minutes
    /// </summary>
    ThirtyMinutes,

    /// <summary>
    /// 60 minutes (1 hour)
    /// </summary>
    SixtyMinutes,

    /// <summary>
    /// Daily
    /// </summary>
    Day,

    /// <summary>
    /// Weekly
    /// </summary>
    Week,

    /// <summary>
    /// Monthly
    /// </summary>
    Month
}
