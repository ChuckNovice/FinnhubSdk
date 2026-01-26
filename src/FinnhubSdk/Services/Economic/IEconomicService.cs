using FinnhubSdk.Models.Economic;

namespace FinnhubSdk.Services.Economic;

/// <summary>
/// Service interface for accessing economic data from Finnhub API
/// </summary>
public interface IEconomicService
{
    /// <summary>
    /// Gets economic data for a specific indicator code
    /// </summary>
    /// <param name="code">The economic indicator code (e.g., MA-USA-656880)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Economic data with time series values</returns>
    /// <exception cref="ArgumentException">Thrown when code is null or whitespace</exception>
    Task<EconomicData?> GetEconomicDataAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available economic indicator codes
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available economic indicator codes</returns>
    Task<IReadOnlyList<EconomicCode>> GetEconomicCodesAsync(CancellationToken cancellationToken = default);
}
