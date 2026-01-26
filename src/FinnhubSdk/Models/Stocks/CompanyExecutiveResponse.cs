namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Internal response model for company executives from Finnhub API
/// </summary>
internal sealed class CompanyExecutiveResponse
{
    /// <summary>
    /// Company symbol
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// List of company executives
    /// </summary>
    public CompanyExecutive[] Executive { get; set; } = Array.Empty<CompanyExecutive>();
}
