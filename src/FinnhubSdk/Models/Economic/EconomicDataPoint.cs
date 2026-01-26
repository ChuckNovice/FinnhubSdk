namespace FinnhubSdk.Models.Economic;

/// <summary>
/// Represents a single data point in economic time series data
/// </summary>
public sealed class EconomicDataPoint
{
    /// <summary>
    /// Gets or sets the date of the data point
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value of the economic indicator
    /// </summary>
    public decimal Value { get; set; }
}
