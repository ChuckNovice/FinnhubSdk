namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Internal response wrapper for insider sentiment API.
/// </summary>
internal sealed class InsiderSentimentResponse
{
    /// <summary>
    /// Gets or sets the stock symbol.
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the insider sentiment data entries.
    /// </summary>
    public InsiderSentimentEntry[] Data { get; set; } = Array.Empty<InsiderSentimentEntry>();
}
