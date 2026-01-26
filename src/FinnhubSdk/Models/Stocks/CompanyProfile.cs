namespace FinnhubSdk.Models.Stocks;

using System.Text.Json.Serialization;

/// <summary>
/// Company profile information
/// </summary>
public sealed class CompanyProfile
{
    /// <summary>
    /// Country of company's headquarters
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Currency used in company financials
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Exchange where the stock is listed
    /// </summary>
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    /// IPO date
    /// </summary>
    public string Ipo { get; set; } = string.Empty;

    /// <summary>
    /// Market capitalization
    /// </summary>
    public decimal MarketCapitalization { get; set; }

    /// <summary>
    /// Company name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Company phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Number of outstanding shares
    /// </summary>
    public decimal ShareOutstanding { get; set; }

    /// <summary>
    /// Stock ticker symbol
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    /// Company website URL
    /// </summary>
    [JsonPropertyName("weburl")]
    public string WebUrl { get; set; } = string.Empty;

    /// <summary>
    /// Company logo URL
    /// </summary>
    public string Logo { get; set; } = string.Empty;

    /// <summary>
    /// Industry classification
    /// </summary>
    [JsonPropertyName("finnhubIndustry")]
    public string Industry { get; set; } = string.Empty;
}
