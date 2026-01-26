namespace FinnhubSdk.Configuration;

using Microsoft.Extensions.Options;

/// <summary>
/// Validator for <see cref="FinnhubOptions"/>
/// </summary>
internal class FinnhubOptionsValidator : IValidateOptions<FinnhubOptions>
{
    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, FinnhubOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return ValidateOptionsResult.Fail("Finnhub API key is required");
        }

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri) ||
            (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
        {
            return ValidateOptionsResult.Fail($"Invalid base URL: {options.BaseUrl}");
        }

        if (!Uri.TryCreate(options.WebSocketUrl, UriKind.Absolute, out var wsUri) ||
            (wsUri.Scheme != "ws" && wsUri.Scheme != "wss"))
        {
            return ValidateOptionsResult.Fail($"Invalid WebSocket URL: {options.WebSocketUrl}");
        }

        if (options.Timeout < TimeSpan.FromSeconds(1) || options.Timeout > TimeSpan.FromMinutes(5))
        {
            return ValidateOptionsResult.Fail("Timeout must be between 1 second and 5 minutes");
        }

        if (options.MaxRetries is < 0 or > 10)
        {
            return ValidateOptionsResult.Fail("Max retries must be between 0 and 10");
        }

        if (options.RetryDelay < TimeSpan.FromMilliseconds(100) || options.RetryDelay > TimeSpan.FromMinutes(1))
        {
            return ValidateOptionsResult.Fail("RetryDelay must be between 100 milliseconds and 1 minute");
        }

        return ValidateOptionsResult.Success;
    }
}
