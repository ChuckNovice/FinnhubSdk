using FinnhubSdk.Configuration;
using Microsoft.Extensions.Options;

namespace FinnhubSdk.Infrastructure.Handlers;

/// <summary>
/// HTTP message handler that adds Finnhub API authentication to requests
/// </summary>
internal class FinnhubAuthenticationHandler : DelegatingHandler
{
    private readonly FinnhubOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubAuthenticationHandler"/> class
    /// </summary>
    /// <param name="options">Finnhub configuration options</param>
    public FinnhubAuthenticationHandler(IOptions<FinnhubOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Add authentication based on configured method
        if (_options.AuthMethod == AuthenticationMethod.Header)
        {
            // Add API key as X-Finnhub-Token header
            request.Headers.Add("X-Finnhub-Token", _options.ApiKey);
        }
        else
        {
            // Add API key as query string parameter
            var uriBuilder = new UriBuilder(request.RequestUri!);
            var query = string.IsNullOrEmpty(uriBuilder.Query)
                ? $"?token={Uri.EscapeDataString(_options.ApiKey)}"
                : $"{uriBuilder.Query}&token={Uri.EscapeDataString(_options.ApiKey)}";

            uriBuilder.Query = query.TrimStart('?');
            request.RequestUri = uriBuilder.Uri;
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
