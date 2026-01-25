using System.Text.Json.Serialization;

namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Internal response model for symbol search from Finnhub API
/// </summary>
internal sealed class SymbolSearchResponse
{
    /// <summary>
    /// Number of results
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// List of matching symbols
    /// </summary>
    [JsonPropertyName("result")]
    public StockSymbol[] Result { get; set; } = Array.Empty<StockSymbol>();
}
