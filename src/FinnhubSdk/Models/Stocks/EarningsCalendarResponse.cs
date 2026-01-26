namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Internal response model for earnings calendar from Finnhub API
/// </summary>
internal sealed class EarningsCalendarResponse
{
    /// <summary>
    /// List of earnings releases
    /// </summary>
    public EarningsCalendarEntry[] EarningsCalendar { get; set; } = [];
}
