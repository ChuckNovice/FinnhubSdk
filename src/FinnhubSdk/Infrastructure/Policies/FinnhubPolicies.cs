namespace FinnhubSdk.Infrastructure.Policies;

using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

/// <summary>
/// Polly resilience policies for Finnhub API requests
/// </summary>
internal static class FinnhubPolicies
{
    /// <summary>
    /// Creates a retry policy with exponential backoff for transient errors
    /// </summary>
    /// <param name="maxRetries">Maximum number of retry attempts</param>
    /// <param name="initialDelay">Initial delay between retries</param>
    /// <param name="logger">Optional logger for retry events</param>
    /// <returns>Retry policy</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(
        int maxRetries = 3,
        TimeSpan? initialDelay = null,
        ILogger? logger = null)
    {
        var delay = initialDelay ?? TimeSpan.FromSeconds(1);

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => TimeSpan.FromMilliseconds(delay.TotalMilliseconds * Math.Pow(2, retryAttempt - 1)),
                onRetry: (outcome, timespan, retryCount, context) => logger?.LogWarning(
                        "Request failed with {StatusCode}. Waiting {Delay}ms before retry {Retry}/{MaxRetries}",
                        outcome.Result?.StatusCode,
                        timespan.TotalMilliseconds,
                        retryCount,
                        maxRetries));
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
                onBreak: (outcome, duration) => logger?.LogWarning(
                        "Circuit breaker opened for {Duration}s after {Exceptions} consecutive failures",
                        duration.TotalSeconds,
                        exceptionsBeforeBreaking),
                onReset: () => logger?.LogInformation("Circuit breaker reset"),
                onHalfOpen: () => logger?.LogInformation("Circuit breaker half-open, testing if service recovered"));
    }

    /// <summary>
    /// Creates a timeout policy for HTTP requests
    /// </summary>
    /// <param name="timeout">Request timeout duration</param>
    /// <param name="logger">Optional logger for timeout events</param>
    /// <returns>Timeout policy</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(
        TimeSpan? timeout = null,
        ILogger? logger = null)
    {
        var actualTimeout = timeout ?? TimeSpan.FromSeconds(30);

        return Policy.TimeoutAsync<HttpResponseMessage>(
            actualTimeout,
            onTimeoutAsync: (context, timeoutDuration, _, _) =>
            {
                logger?.LogWarning(
                    "Request timed out after {Timeout}s",
                    timeoutDuration.TotalSeconds);
                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Creates a combined policy with retry, circuit breaker, and timeout
    /// </summary>
    /// <param name="maxRetries">Maximum retry attempts</param>
    /// <param name="initialDelay">Initial retry delay</param>
    /// <param name="timeout">Request timeout</param>
    /// <param name="logger">Optional logger</param>
    /// <returns>Combined policy</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy(
        int maxRetries = 3,
        TimeSpan? initialDelay = null,
        TimeSpan? timeout = null,
        ILogger? logger = null)
    {
        return Policy.WrapAsync(
            GetRetryPolicy(maxRetries, initialDelay, logger),
            GetCircuitBreakerPolicy(logger: logger),
            GetTimeoutPolicy(timeout, logger));
    }
}
