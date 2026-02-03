using FinnhubSdk.Clients;
using FinnhubSdk.Configuration;

namespace FinnhubSdk;

/// <summary>
/// Thread-safe caching factory for Finnhub clients.
/// Uses a dedicated MemoryCache with configurable expiration and eviction policies.
/// Automatically disposes client WebSocket connections when entries are evicted.
/// </summary>
public interface ICachedFinnhubClientFactory
{
    /// <summary>
    /// Gets an existing client from cache or creates a new one.
    /// Thread-safe: concurrent calls with the same key return the same instance.
    /// </summary>
    /// <param name="apiKey">The Finnhub API key used as the cache key</param>
    /// <returns>A cached or newly created Finnhub client instance</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the factory has been disposed</exception>
    IFinnhubClient GetOrCreateClient(string apiKey);

    /// <summary>
    /// Gets an existing client from cache or creates a new one with custom options.
    /// The cache key is derived from the ApiKey in options.
    /// </summary>
    /// <param name="configureOptions">Action to configure the client options, including API key</param>
    /// <returns>A cached or newly created Finnhub client instance</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the factory has been disposed</exception>
    IFinnhubClient GetOrCreateClient(Action<FinnhubOptions> configureOptions);

    /// <summary>
    /// Removes a client from cache and disposes its WebSocket connection.
    /// Call when a user logs out or their API key is revoked.
    /// Safe to call with non-existent keys.
    /// </summary>
    /// <param name="apiKey">The API key of the client to remove</param>
    ValueTask RemoveAsync(string apiKey);

    /// <summary>
    /// Removes all clients from cache and disposes their WebSocket connections.
    /// </summary>
    ValueTask ClearAsync();
}
