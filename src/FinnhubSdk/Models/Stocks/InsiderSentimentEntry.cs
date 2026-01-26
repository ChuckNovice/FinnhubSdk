namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Monthly insider sentiment data for a stock.
/// </summary>
public sealed class InsiderSentimentEntry
{
    /// <summary>
    /// Gets or sets the year.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Gets or sets the month (1-12).
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Gets or sets the net change in insider shares (buy - sell).
    /// </summary>
    public long Change { get; set; }

    /// <summary>
    /// Gets or sets the Monthly Share Purchase Ratio (MSPR).
    /// MSPR = (buy - sell) / total. Range: -100 to 100.
    /// Positive values indicate net buying, negative indicate net selling.
    /// </summary>
    public decimal Mspr { get; set; }
}
