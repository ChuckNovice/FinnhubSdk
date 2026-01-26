namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Earnings release information for a company
/// </summary>
public sealed class EarningsCalendarEntry
{
    /// <summary>
    /// Date when earnings are announced (YYYY-MM-DD format)
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Actual earnings per share result
    /// </summary>
    public decimal? EpsActual { get; set; }

    /// <summary>
    /// Estimated earnings per share
    /// </summary>
    public decimal? EpsEstimate { get; set; }

    /// <summary>
    /// Timing indicator: "bmo" (before market open), "amc" (after market close), or "dmh" (during market hours)
    /// </summary>
    public string Hour { get; set; } = string.Empty;

    /// <summary>
    /// Fiscal quarter number (1-4)
    /// </summary>
    public int? Quarter { get; set; }

    /// <summary>
    /// Actual revenue (in millions)
    /// </summary>
    public decimal? RevenueActual { get; set; }

    /// <summary>
    /// Estimated revenue (in millions)
    /// </summary>
    public decimal? RevenueEstimate { get; set; }

    /// <summary>
    /// Company stock symbol
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Fiscal year
    /// </summary>
    public int? Year { get; set; }
}
