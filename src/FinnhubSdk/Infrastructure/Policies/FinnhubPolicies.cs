using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace FinnhubSdk.Infrastructure.Policies;

/// <summary>
/// Polly resilience policies for Finnhub API requests
/// </summary>
internal static class FinnhubPolicies
{
    /// <summary>
    /// Creates a retry policy with exponential backoff for transient errors
    /// </summary>
    /// <param name="maxRetries">Maximum number of retry attempts</param>
    /// <param name="initialDelayMs">Initial delay in milliseconds</param>
    /// <param name="logger">Optional logger for retry events</param>
    /// <returns>Retry policy</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(
        int maxRetries = 3,
        int initialDelayMs = 1000,
        ILogger? logger = null)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => TimeSpan.FromMilliseconds(initialDelayMs * Math.Pow(2, retryAttempt - 1)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    logger?.LogWarning(
                        "Request failed with {StatusCode}. Waiting {Delay}ms before retry {Retry}/{MaxRetries}",
                        outcome.Result?.StatusCode,
                        timespan.TotalMilliseconds,
                        retryCount,
                        maxRetries);
                });
    }

    /// <summary>
    /// Creates a circuit breaker policy to prevent cascading failures
    /// </summary>
    /// <param name="exceptionsBeforeBreaking">Number of exceptions before breaking the circuit</param>
    /// <param name="durationOfBreakSeconds">Duration to keep the circuit broken</param>
    /// <param name="logger">Optional logger for circuit breaker events</param>
    /// <returns>Circuit breaker policy</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
        int exceptionsBeforeBreaking = 5,
        int durationOfBreakSeconds = 30,
        ILogger? logger = null)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                exceptionsBeforeBreaking,
                TimeSpan.FromSeconds(durationOfBreakSeconds),
                onBreak: (outcome, duration) =>
                {
                    logger?.LogWarning(
                        "Circuit breaker opened for {Duration}s after {Exceptions} consecutive failures",
                        duration.TotalSeconds,
                        exceptionsBeforeBreaking);
                },
                onReset: () =>
                {
                    logger?.LogInformation("Circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    logger?.LogInformation("Circuit breaker half-open, testing if service recovered");
                });
    }

    /// <summary>
    /// Creates a timeout policy for HTTP requests
    /// </summary>
    /// <param name="timeoutSeconds">Timeout in seconds</param>
    /// <param name="logger">Optional logger for timeout events</param>
    /// <returns>Timeout policy</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(
        int timeoutSeconds = 30,
        ILogger? logger = null)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(timeoutSeconds),
            onTimeoutAsync: (context, timeout, _, _) =>
            {
                logger?.LogWarning(
                    "Request timed out after {Timeout}s",
                    timeout.TotalSeconds);
                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Creates a combined policy with retry, circuit breaker, and timeout
    /// </summary>
    /// <param name="maxRetries">Maximum retry attempts</param>
    /// <param name="initialDelayMs">Initial retry delay</param>
    /// <param name="timeoutSeconds">Request timeout</param>
    /// <param name="logger">Optional logger</param>
    /// <returns>Combined policy</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy(
        int maxRetries = 3,
        int initialDelayMs = 1000,
        int timeoutSeconds = 30,
        ILogger? logger = null)
    {
        return Policy.WrapAsync(
            GetRetryPolicy(maxRetries, initialDelayMs, logger),
            GetCircuitBreakerPolicy(logger: logger),
            GetTimeoutPolicy(timeoutSeconds, logger));
    }
}
