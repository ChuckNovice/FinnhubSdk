namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Internal response wrapper for insider transactions API.
/// </summary>
internal sealed class InsiderTransactionResponse
{
    /// <summary>
    /// Gets or sets the stock symbol.
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the insider transactions.
    /// </summary>
    public InsiderTransaction[] Data { get; set; } = Array.Empty<InsiderTransaction>();
}
