namespace FinnhubSdk.Exceptions;

/// <summary>
/// Exception thrown when API rate limits are exceeded (HTTP 429)
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FinnhubRateLimitException"/> class
/// </remarks>
/// <param name="message">The error message</param>
/// <param name="retryAfter">When to retry the request</param>
public class FinnhubRateLimitException(string message, DateTime? retryAfter = null) : FinnhubException(message)
{
    /// <summary>
    /// Gets the time when the client should retry the request
    /// </summary>
    public DateTime? RetryAfter { get; set; } = retryAfter;

    /// <summary>
    /// Gets the number of remaining requests if available
    /// </summary>
    public int? RemainingRequests { get; set; }

    /// <summary>
    /// Gets the rate limit for this tier if available
    /// </summary>
    public int? RateLimitLimit { get; set; }
}
