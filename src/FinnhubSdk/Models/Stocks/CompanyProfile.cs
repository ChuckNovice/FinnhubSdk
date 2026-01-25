using System.Text.Json.Serialization;

namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Company profile information
/// </summary>
public sealed class CompanyProfile
{
    /// <summary>
    /// Country of company's headquarters
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Currency used in company financials
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Exchange where the stock is listed
    /// </summary>
    [JsonPropertyName("exchange")]
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    /// IPO date
    /// </summary>
    [JsonPropertyName("ipo")]
    public string Ipo { get; set; } = string.Empty;

    /// <summary>
    /// Market capitalization
    /// </summary>
    [JsonPropertyName("marketCapitalization")]
    public decimal MarketCapitalization { get; set; }

    /// <summary>
    /// Company name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Company phone number
    /// </summary>
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Number of outstanding shares
    /// </summary>
    [JsonPropertyName("shareOutstanding")]
    public decimal ShareOutstanding { get; set; }

    /// <summary>
    /// Stock ticker symbol
    /// </summary>
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    /// Company website URL
    /// </summary>
    [JsonPropertyName("weburl")]
    public string WebUrl { get; set; } = string.Empty;

    /// <summary>
    /// Company logo URL
    /// </summary>
    [JsonPropertyName("logo")]
    public string Logo { get; set; } = string.Empty;

    /// <summary>
    /// Industry classification
    /// </summary>
    [JsonPropertyName("finnhubIndustry")]
    public string Industry { get; set; } = string.Empty;
}
