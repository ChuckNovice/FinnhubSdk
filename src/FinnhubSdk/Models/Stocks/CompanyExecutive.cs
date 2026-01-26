namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Information about a company executive or board member
/// </summary>
public sealed class CompanyExecutive
{
    /// <summary>
    /// Executive's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Position/title within the organization
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Total compensation amount
    /// </summary>
    public long? Compensation { get; set; }

    /// <summary>
    /// Compensation currency (e.g., "USD")
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Executive's age
    /// </summary>
    public int? Age { get; set; }

    /// <summary>
    /// Gender
    /// </summary>
    public string Sex { get; set; } = string.Empty;

    /// <summary>
    /// Year appointed to executive or board role
    /// </summary>
    public string Since { get; set; } = string.Empty;
}
