using Microsoft.Extensions.Caching.Memory;

namespace FinnhubSdk.Configuration;

/// <summary>
/// Configuration options for <see cref="ICachedFinnhubClientFactory"/>.
/// Exposes standard .NET caching options for both the cache instance and individual entries.
/// </summary>
public class CachedFinnhubClientFactoryOptions
{
    /// <summary>
    /// Options for the dedicated MemoryCache instance.
    /// This cache is isolated from the application's IMemoryCache.
    /// Configure SizeLimit, CompactionPercentage, ExpirationScanFrequency, etc.
    /// </summary>
    public MemoryCacheOptions CacheOptions { get; set; } = new()
    {
        SizeLimit = 1000
    };

    /// <summary>
    /// Options applied to each cache entry.
    /// Configure SlidingExpiration, AbsoluteExpiration, Priority, Size, etc.
    /// </summary>
    public MemoryCacheEntryOptions EntryOptions { get; set; } = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(30),
        Size = 1
    };
}
