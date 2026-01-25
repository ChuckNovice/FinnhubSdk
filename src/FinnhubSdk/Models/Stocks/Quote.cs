using System.Text.Json.Serialization;

namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Real-time stock quote data
/// </summary>
public sealed class Quote
{
    /// <summary>
    /// Current price
    /// </summary>
    [JsonPropertyName("c")]
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// High price of the day
    /// </summary>
    [JsonPropertyName("h")]
    public decimal High { get; set; }

    /// <summary>
    /// Low price of the day
    /// </summary>
    [JsonPropertyName("l")]
    public decimal Low { get; set; }

    /// <summary>
    /// Open price of the day
    /// </summary>
    [JsonPropertyName("o")]
    public decimal Open { get; set; }

    /// <summary>
    /// Previous close price
    /// </summary>
    [JsonPropertyName("pc")]
    public decimal PreviousClose { get; set; }

    /// <summary>
    /// Change from previous close
    /// </summary>
    [JsonPropertyName("d")]
    public decimal Change { get; set; }

    /// <summary>
    /// Percent change from previous close
    /// </summary>
    [JsonPropertyName("dp")]
    public decimal PercentChange { get; set; }

    /// <summary>
    /// Unix timestamp of the quote
    /// </summary>
    [JsonPropertyName("t")]
    public long Timestamp { get; set; }
}
