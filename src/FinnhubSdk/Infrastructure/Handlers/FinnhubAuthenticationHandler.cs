using FinnhubSdk.Configuration;
using Microsoft.Extensions.Options;

namespace FinnhubSdk.Infrastructure.Handlers;

/// <summary>
/// HTTP message handler that adds Finnhub API authentication to requests.
/// Supports both DI-registered clients (via global options) and factory-created clients (via per-request options).
/// </summary>
internal class FinnhubAuthenticationHandler : DelegatingHandler
{
    private readonly FinnhubOptions? _globalOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubAuthenticationHandler"/> class
    /// </summary>
    /// <param name="options">Finnhub configuration options (optional for factory-created clients)</param>
    public FinnhubAuthenticationHandler(IOptions<FinnhubOptions>? options = null)
    {
        _globalOptions = options?.Value;
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Check per-request options first (for factory-created clients)
        if (request.Options.TryGetValue(FinnhubRequestOptions.ApiKey, out var apiKey) &&
            !string.IsNullOrEmpty(apiKey))
        {
            var authMethod = AuthenticationMethod.Header;
            if (request.Options.TryGetValue(FinnhubRequestOptions.AuthMethod, out var method))
            {
                authMethod = method;
            }

            AddAuthentication(request, apiKey, authMethod);
        }
        // Fall back to global options (for DI-registered clients)
        else if (_globalOptions != null && !string.IsNullOrEmpty(_globalOptions.ApiKey))
        {
            AddAuthentication(request, _globalOptions.ApiKey, _globalOptions.AuthMethod);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static void AddAuthentication(HttpRequestMessage request, string apiKey, AuthenticationMethod authMethod)
    {
        if (authMethod == AuthenticationMethod.Header)
        {
            // Add API key as X-Finnhub-Token header
            request.Headers.Add("X-Finnhub-Token", apiKey);
        }
        else
        {
            // Add API key as query string parameter
            var uriBuilder = new UriBuilder(request.RequestUri!);
            var query = string.IsNullOrEmpty(uriBuilder.Query)
                ? $"?token={Uri.EscapeDataString(apiKey)}"
                : $"{uriBuilder.Query}&token={Uri.EscapeDataString(apiKey)}";

            uriBuilder.Query = query.TrimStart('?');
            request.RequestUri = uriBuilder.Uri;
        }
    }
}
