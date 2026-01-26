using FinnhubSdk.Clients;
using FinnhubSdk.Models.Economic;
using Microsoft.Extensions.Logging;

namespace FinnhubSdk.Services.Economic;

/// <summary>
/// Implementation of <see cref="IEconomicService"/> for accessing economic data
/// </summary>
internal sealed class EconomicService : IEconomicService
{
    private readonly FinnhubHttpClient _httpClient;
    private readonly ILogger<EconomicService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EconomicService"/> class
    /// </summary>
    /// <param name="httpClient">HTTP client for API requests</param>
    /// <param name="logger">Logger instance</param>
    public EconomicService(FinnhubHttpClient httpClient, ILogger<EconomicService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<EconomicData?> GetEconomicDataAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code cannot be null or whitespace", nameof(code));
        }

        _logger.LogDebug("Getting economic data for code: {Code}", code);

        var response = await _httpClient.GetAsync<EconomicData>(
            $"economic?code={Uri.EscapeDataString(code)}",
            null,
            cancellationToken);

        if (response == null)
        {
            _logger.LogWarning("No economic data returned for code: {Code}", code);
            return null;
        }

        _logger.LogDebug("Retrieved {Count} data points for economic code: {Code}", response.Data.Count, code);

        return response;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<EconomicCode>> GetEconomicCodesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all economic indicator codes");

        var response = await _httpClient.GetAsync<List<EconomicCode>>(
            "economic/code",
            null,
            cancellationToken);

        if (response == null)
        {
            _logger.LogWarning("No economic codes returned");
            return [];
        }

        _logger.LogDebug("Retrieved {Count} economic indicator codes", response.Count);

        return response;
    }
}
