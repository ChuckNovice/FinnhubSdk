using System.Text.Json.Serialization;

namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Basic financial metrics for a company
/// </summary>
public sealed class BasicFinancials
{
    /// <summary>
    /// Stock symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Type of metrics returned (e.g., "all", "price", "valuation")
    /// </summary>
    [JsonPropertyName("metricType")]
    public string MetricType { get; set; } = string.Empty;

    /// <summary>
    /// Financial metric values
    /// </summary>
    [JsonPropertyName("metric")]
    public MetricData? Metric { get; set; }
}
