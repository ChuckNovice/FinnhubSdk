using System.Collections.Concurrent;
using FinnhubSdk.Clients;
using FinnhubSdk.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FinnhubSdk;

/// <summary>
/// Thread-safe caching factory for Finnhub clients.
/// Uses a dedicated MemoryCache instance with configurable expiration and eviction policies.
/// Automatically disposes client WebSocket connections when entries are evicted.
/// </summary>
internal sealed class CachedFinnhubClientFactory : ICachedFinnhubClientFactory, IAsyncDisposable
{
    private readonly IFinnhubClientFactory _factory;
    private readonly MemoryCache _cache;
    private readonly CachedFinnhubClientFactoryOptions _options;
    private readonly object _createLock = new();
    private readonly ConcurrentDictionary<string, byte> _trackedKeys = new();
    private int _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedFinnhubClientFactory"/> class
    /// </summary>
    /// <param name="factory">Underlying stateless factory</param>
    /// <param name="options">Cache configuration options</param>
    public CachedFinnhubClientFactory(
        IFinnhubClientFactory factory,
        IOptions<CachedFinnhubClientFactoryOptions> options)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _cache = new MemoryCache(_options.CacheOptions);
    }

    /// <inheritdoc/>
    public IFinnhubClient GetOrCreateClient(string apiKey)
    {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);

        // Fast path: cache hit (MemoryCache.TryGetValue is thread-safe)
        if (_cache.TryGetValue(apiKey, out IFinnhubClient? cached))
        {
            return cached!;
        }

        // Slow path: acquire lock to prevent duplicate client creation
        lock (_createLock)
        {
            // Double-check after acquiring lock (another thread may have created it)
            if (_cache.TryGetValue(apiKey, out cached))
            {
                return cached!;
            }

            var client = _factory.CreateClient(apiKey);

            // Track key BEFORE adding to cache
            // If eviction happens immediately after Set(), OnClientEvicted will find the key
            _trackedKeys.TryAdd(apiKey, 0);

            var entryOptions = BuildEntryOptions();
            _cache.Set(apiKey, client, entryOptions);

            // Edge case: If entry was evicted immediately (size limit, memory pressure),
            // OnClientEvicted already ran and disposed the client.
            // Check if still in cache; if not, retry with a new client.
            if (!_cache.TryGetValue(apiKey, out _))
            {
                // Client was evicted and disposed immediately - remove stale tracking and retry
                _trackedKeys.TryRemove(apiKey, out _);
                return GetOrCreateClient(apiKey);
            }

            return client;
        }
    }

    /// <inheritdoc/>
    public IFinnhubClient GetOrCreateClient(Action<FinnhubOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);
        ObjectDisposedException.ThrowIf(_disposed != 0, this);

        // Extract the API key to use as cache key
        var options = new FinnhubOptions();
        configureOptions(options);

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new InvalidOperationException("API key must be configured");
        }

        // Fast path: cache hit
        if (_cache.TryGetValue(options.ApiKey, out IFinnhubClient? cached))
        {
            return cached!;
        }

        // Slow path: create with full options
        lock (_createLock)
        {
            if (_cache.TryGetValue(options.ApiKey, out cached))
            {
                return cached!;
            }

            var client = _factory.CreateClient(configureOptions);
            _trackedKeys.TryAdd(options.ApiKey, 0);

            var entryOptions = BuildEntryOptions();
            _cache.Set(options.ApiKey, client, entryOptions);

            if (!_cache.TryGetValue(options.ApiKey, out _))
            {
                _trackedKeys.TryRemove(options.ApiKey, out _);
                return GetOrCreateClient(configureOptions);
            }

            return client;
        }
    }

    /// <inheritdoc/>
    public ValueTask RemoveAsync(string apiKey)
    {
        _cache.Remove(apiKey); // Triggers OnClientEvicted
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask ClearAsync()
    {
        // Snapshot keys and remove each (triggers OnClientEvicted for each)
        foreach (var key in _trackedKeys.Keys.ToArray())
        {
            _cache.Remove(key);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return; // Already disposed
        }

        // Collect all clients before clearing (so we can await their disposal)
        var clients = new List<IFinnhubClient>();
        foreach (var key in _trackedKeys.Keys.ToArray())
        {
            if (_cache.TryGetValue(key, out IFinnhubClient? client) && client is not null)
            {
                clients.Add(client);
            }
        }

        // Clear cache (triggers OnClientEvicted, but we'll also dispose manually)
        foreach (var key in _trackedKeys.Keys.ToArray())
        {
            _cache.Remove(key);
        }

        // Await disposal of all clients (DisposeAsync is idempotent, so double-dispose is safe)
        var disposalTasks = clients.Select(c => DisposeClientSafelyAsync(c));
        await Task.WhenAll(disposalTasks);

        _cache.Dispose();
    }

    private MemoryCacheEntryOptions BuildEntryOptions()
    {
        var entryOptions = new MemoryCacheEntryOptions()
            .SetSize(_options.EntryOptions.Size ?? 1)
            .RegisterPostEvictionCallback(OnClientEvicted);

        if (_options.EntryOptions.SlidingExpiration.HasValue)
        {
            entryOptions.SetSlidingExpiration(_options.EntryOptions.SlidingExpiration.Value);
        }

        if (_options.EntryOptions.AbsoluteExpirationRelativeToNow.HasValue)
        {
            entryOptions.SetAbsoluteExpiration(_options.EntryOptions.AbsoluteExpirationRelativeToNow.Value);
        }

        entryOptions.SetPriority(_options.EntryOptions.Priority);

        return entryOptions;
    }

    /// <summary>
    /// Called by MemoryCache when entry is evicted (expiration, size limit, manual removal).
    /// MemoryCache does NOT call Dispose on evicted items - disposal happens here.
    /// </summary>
    private void OnClientEvicted(object key, object? value, EvictionReason reason, object? state)
    {
        var apiKey = (string)key;
        _trackedKeys.TryRemove(apiKey, out _);

        // Fire-and-forget disposal - we don't block the eviction
        // FinnhubWebSocketClient.DisposeAsync() must be idempotent
        if (value is IFinnhubClient client)
        {
            _ = DisposeClientSafelyAsync(client);
        }
    }

    private static async Task DisposeClientSafelyAsync(IFinnhubClient client)
    {
        try
        {
            await client.WebSocket.DisposeAsync();
        }
        catch
        {
            // Swallow disposal errors - nothing we can do here
        }
    }
}
