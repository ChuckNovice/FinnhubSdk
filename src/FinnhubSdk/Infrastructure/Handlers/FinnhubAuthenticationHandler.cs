namespace FinnhubSdk.Infrastructure.Handlers;

using FinnhubSdk.Configuration;
using Microsoft.Extensions.Options;

/// <summary>
/// HTTP message handler that adds Finnhub API authentication to requests
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FinnhubAuthenticationHandler"/> class
/// </remarks>
/// <param name="options">Finnhub configuration options</param>
internal class FinnhubAuthenticationHandler(IOptions<FinnhubOptions> options) : DelegatingHandler
{
    private readonly FinnhubOptions _options = options.Value;

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
