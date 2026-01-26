namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Internal response model for symbol search from Finnhub API
/// </summary>
internal sealed class SymbolSearchResponse
{
    /// <summary>
    /// Number of results
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// List of matching symbols
    /// </summary>
    public StockSymbol[] Result { get; set; } = [];
}
