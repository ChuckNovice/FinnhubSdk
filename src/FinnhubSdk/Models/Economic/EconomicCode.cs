namespace FinnhubSdk.Models.Economic;

/// <summary>
/// Represents an economic indicator code from Finnhub
/// </summary>
public sealed class EconomicCode
{
    /// <summary>
    /// Gets or sets the economic indicator code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country associated with this indicator
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name/description of the economic indicator
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit of measurement for this indicator
    /// </summary>
    public string Unit { get; set; } = string.Empty;
}
