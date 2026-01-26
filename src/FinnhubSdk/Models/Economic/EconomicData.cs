namespace FinnhubSdk.Models.Economic;

/// <summary>
/// Represents economic data for a specific indicator code
/// </summary>
public sealed class EconomicData
{
    /// <summary>
    /// Gets or sets the economic indicator code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the time series data points
    /// </summary>
    public IReadOnlyList<EconomicDataPoint> Data { get; set; } = [];
}
