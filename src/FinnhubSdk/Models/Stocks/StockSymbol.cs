using System.Text.Json.Serialization;

namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Stock symbol search result
/// </summary>
public sealed class StockSymbol
{
    /// <summary>
    /// Symbol description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Display symbol name
    /// </summary>
    [JsonPropertyName("displaySymbol")]
    public string DisplaySymbol { get; set; } = string.Empty;

    /// <summary>
    /// Unique symbol
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Security type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}
