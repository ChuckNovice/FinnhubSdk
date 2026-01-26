namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// SEC filing data for a company.
/// </summary>
public sealed class SecFiling
{
    /// <summary>
    /// Gets or sets the accession number (unique identifier for the filing).
    /// </summary>
    public string AccessNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stock symbol.
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CIK (Central Index Key) number.
    /// </summary>
    public string Cik { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the form type (e.g., "10-K", "10-Q", "8-K").
    /// </summary>
    public string Form { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filing date (YYYY-MM-DD).
    /// </summary>
    public string FiledDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the accepted datetime.
    /// </summary>
    public string AcceptedDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to the filing on SEC EDGAR.
    /// </summary>
    public string ReportUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to the filing document.
    /// </summary>
    public string FilingUrl { get; set; } = string.Empty;
}
