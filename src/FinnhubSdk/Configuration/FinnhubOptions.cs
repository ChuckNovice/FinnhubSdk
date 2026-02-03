using System.ComponentModel.DataAnnotations;

namespace FinnhubSdk.Configuration;

/// <summary>
/// Shared configuration options for Finnhub clients (URLs, timeouts, retry policies).
/// Used as the base configuration for both single-client DI registration and factory-created clients.
/// </summary>
public class FinnhubClientOptions
{
    /// <summary>
    /// Gets or sets the base URL for the Finnhub REST API
    /// </summary>
    [Url]
    public string BaseUrl { get; set; } = "https://finnhub.io/api/v1";

    /// <summary>
    /// Gets or sets the WebSocket URL for real-time data.
    /// Must be a valid WebSocket URL (ws:// or wss://).
    /// </summary>
    public string WebSocketUrl { get; set; } = "wss://ws.finnhub.io";

    /// <summary>
    /// Gets or sets the authentication method (Header or QueryString)
    /// </summary>
    public AuthenticationMethod AuthMethod { get; set; } = AuthenticationMethod.Header;

    /// <summary>
    /// Gets or sets the request timeout.
    /// Must be between 1 second and 5 minutes.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum number of retry attempts
    /// </summary>
    [Range(0, 10)]
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial delay between retry attempts.
    /// Must be between 100 milliseconds and 1 minute.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets whether to enable detailed logging
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Gets or sets the rate limit strategy
    /// </summary>
    public RateLimitStrategy RateLimitStrategy { get; set; } = RateLimitStrategy.ThrowException;
}

/// <summary>
/// Full configuration options for the Finnhub SDK, including API key.
/// Used for single-client DI registration via AddFinnhub().
/// </summary>
public class FinnhubOptions : FinnhubClientOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "Finnhub";

    /// <summary>
    /// Gets or sets the Finnhub API key (required)
    /// </summary>
    [Required(ErrorMessage = "Finnhub API key is required")]
    public string ApiKey { get; set; } = string.Empty;
}

/// <summary>
/// Authentication method for API requests
/// </summary>
public enum AuthenticationMethod
{
    /// <summary>
    /// Use X-Finnhub-Token header
    /// </summary>
    Header,

    /// <summary>
    /// Use ?token= query string parameter
    /// </summary>
    QueryString
}

/// <summary>
/// Strategy for handling rate limits
/// </summary>
public enum RateLimitStrategy
{
    /// <summary>
    /// Throw exception when rate limited
    /// </summary>
    ThrowException,

    /// <summary>
    /// Retry with exponential backoff when rate limited
    /// </summary>
    RetryWithBackoff,

    /// <summary>
    /// Queue requests to stay within rate limits
    /// </summary>
    QueueRequest
}
