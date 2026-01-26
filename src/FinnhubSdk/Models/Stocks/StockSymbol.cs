namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Stock symbol search result
/// </summary>
public sealed class StockSymbol
{
    /// <summary>
    /// Symbol description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Display symbol name
    /// </summary>
    public string DisplaySymbol { get; set; } = string.Empty;

    /// <summary>
    /// Unique symbol
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Security type
    /// </summary>
    public string Type { get; set; } = string.Empty;
}
