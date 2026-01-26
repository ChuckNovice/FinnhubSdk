namespace FinnhubSdk.Infrastructure.Handlers;

using System.Net;
using FinnhubSdk.Configuration;
using FinnhubSdk.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// HTTP message handler that manages rate limiting for Finnhub API requests
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RateLimitHandler"/> class
/// </remarks>
/// <param name="options">Finnhub configuration options</param>
/// <param name="logger">Logger instance</param>
internal class RateLimitHandler(
    IOptions<FinnhubOptions> options,
    ILogger<RateLimitHandler> logger) : DelegatingHandler
{
    private readonly SemaphoreSlim _rateLimiter = new(60, 60);
    private readonly FinnhubOptions _options = options.Value;
    private readonly ILogger<RateLimitHandler> _logger = logger;

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await _rateLimiter.WaitAsync(cancellationToken);

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            // Check for rate limit response (429)
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // IMPORTANT: Check Retry-After header FIRST (API-specified wait time)
                TimeSpan retryDelay;

                if (response.Headers.RetryAfter != null)
                {
                    // Use API-specified retry delay
                    if (response.Headers.RetryAfter.Delta.HasValue)
                    {
                        retryDelay = response.Headers.RetryAfter.Delta.Value;
                        _logger.LogWarning(
                            "Rate limit exceeded. API specified retry after {Delay}",
                            retryDelay);
                    }
                    else if (response.Headers.RetryAfter.Date.HasValue)
                    {
                        retryDelay = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
                        _logger.LogWarning(
                            "Rate limit exceeded. API specified retry at {RetryAt}",
                            response.Headers.RetryAfter.Date.Value);
                    }
                    else
                    {
                        // Fallback to default exponential backoff
                        retryDelay = TimeSpan.FromMinutes(1);
                        _logger.LogWarning(
                            "Rate limit exceeded. Retry-After header present but no delay specified, using default");
                    }
                }
                else
                {
                    // No Retry-After header, use default exponential backoff
                    retryDelay = TimeSpan.FromMinutes(1);
                    _logger.LogWarning(
                        "Rate limit exceeded. No Retry-After header, using default delay of 1 minute");
                }

                throw new FinnhubRateLimitException(
                    "Rate limit exceeded",
                    DateTime.UtcNow.Add(retryDelay))
                {
                    StatusCode = HttpStatusCode.TooManyRequests
                };
            }

            // Track remaining rate limit if headers are present
            if (response.Headers.TryGetValues("X-Ratelimit-Remaining", out var remainingValues))
            {
                if (int.TryParse(remainingValues.FirstOrDefault(), out var remaining))
                {
                    _logger.LogDebug("Rate limit remaining: {Remaining}", remaining);

                    if (remaining < 10)
                    {
                        _logger.LogWarning(
                            "Approaching rate limit. Only {Remaining} requests remaining",
                            remaining);
                    }
                }
            }

            return response;
        }
        finally
        {
            // Release token after delay to maintain rate limit (60 req/min = ~1 req/sec)
            _ = Task.Delay(TimeSpan.FromSeconds(1), cancellationToken)
                .ContinueWith(_ =>
                {
                    try
                    {
                        _rateLimiter.Release();
                    }
                    catch (ObjectDisposedException)
                    {
                        // Handler is being disposed, ignore
                    }
                }, CancellationToken.None);
        }
    }

    /// <summary>
    /// Disposes the rate limiter
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _rateLimiter?.Dispose();
        }

        base.Dispose(disposing);
    }
}
