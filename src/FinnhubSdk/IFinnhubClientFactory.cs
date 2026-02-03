using FinnhubSdk.Clients;
using FinnhubSdk.Configuration;

namespace FinnhubSdk;

/// <summary>
/// Stateless factory for creating Finnhub clients with dynamic API keys.
/// Each call creates a new instance with its own services and WebSocket connection.
/// For caching, use <see cref="ICachedFinnhubClientFactory"/> or implement your own caching strategy.
/// </summary>
public interface IFinnhubClientFactory
{
    /// <summary>
    /// Creates a new Finnhub client with the specified API key.
    /// Uses shared configuration options (BaseUrl, Timeout, etc.) from DI registration.
    /// </summary>
    /// <param name="apiKey">The Finnhub API key for this client</param>
    /// <returns>A new Finnhub client instance</returns>
    /// <exception cref="ArgumentNullException">Thrown when apiKey is null</exception>
    /// <exception cref="ArgumentException">Thrown when apiKey is empty or whitespace</exception>
    IFinnhubClient CreateClient(string apiKey);

    /// <summary>
    /// Creates a new Finnhub client with custom options.
    /// Starts with shared configuration defaults and applies the provided customization.
    /// </summary>
    /// <param name="configureOptions">Action to configure the client options, including API key</param>
    /// <returns>A new Finnhub client instance</returns>
    /// <exception cref="ArgumentNullException">Thrown when configureOptions is null</exception>
    IFinnhubClient CreateClient(Action<FinnhubOptions> configureOptions);
}
