namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Insider transaction data from SEC Form 4 filings.
/// </summary>
public sealed class InsiderTransaction
{
    /// <summary>
    /// Gets or sets the stock symbol.
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the insider.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of shares involved in the transaction.
    /// </summary>
    public long Share { get; set; }

    /// <summary>
    /// Gets or sets the change in shares (positive for buys, negative for sells).
    /// </summary>
    public long Change { get; set; }

    /// <summary>
    /// Gets or sets the filing date (YYYY-MM-DD).
    /// </summary>
    public string FilingDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction date (YYYY-MM-DD).
    /// </summary>
    public string TransactionDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction price per share.
    /// </summary>
    public decimal? TransactionPrice { get; set; }

    /// <summary>
    /// Gets or sets the transaction code (e.g., "P" for purchase, "S" for sale).
    /// </summary>
    public string TransactionCode { get; set; } = string.Empty;
}
