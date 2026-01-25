using Microsoft.Extensions.Options;

namespace FinnhubSdk.Configuration;

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

        if (options.TimeoutSeconds < 1 || options.TimeoutSeconds > 300)
        {
            return ValidateOptionsResult.Fail("Timeout must be between 1 and 300 seconds");
        }

        if (options.MaxRetries < 0 || options.MaxRetries > 10)
        {
            return ValidateOptionsResult.Fail("Max retries must be between 0 and 10");
        }

        return ValidateOptionsResult.Success;
    }
}
