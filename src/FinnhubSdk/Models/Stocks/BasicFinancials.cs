namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Basic financial metrics for a company
/// </summary>
public sealed class BasicFinancials
{
    /// <summary>
    /// Stock symbol
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Type of metrics returned (e.g., "all", "price", "valuation")
    /// </summary>
    public string MetricType { get; set; } = string.Empty;

    /// <summary>
    /// Financial metric values
    /// </summary>
    public MetricData? Metric { get; set; }
}
