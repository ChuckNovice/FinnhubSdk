namespace FinnhubSdk.Clients;

using System.Net;
using System.Text.Json;
using FinnhubSdk.Configuration;
using FinnhubSdk.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Base HTTP client for making requests to the Finnhub API
/// </summary>
internal class FinnhubHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly FinnhubOptions _options;
    private readonly ILogger<FinnhubHttpClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubHttpClient"/> class
    /// </summary>
    /// <param name="httpClient">HTTP client instance</param>
    /// <param name="options">Finnhub configuration options</param>
    /// <param name="logger">Logger instance</param>
    public FinnhubHttpClient(
        HttpClient httpClient,
        IOptions<FinnhubOptions> options,
        ILogger<FinnhubHttpClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// Sends a GET request to the specified endpoint
    /// </summary>
    /// <typeparam name="T">Response type</typeparam>
    /// <param name="endpoint">API endpoint (relative to base URL)</param>
    /// <param name="queryParameters">Optional query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response</returns>
    public virtual async Task<T> GetAsync<T>(
        string endpoint,
        Dictionary<string, string>? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint, queryParameters);

        if (_options.EnableLogging)
        {
            _logger.LogDebug("GET {Url}", url);
        }

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            return await ProcessResponseAsync<T>(response, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for {Endpoint}", endpoint);
            throw new FinnhubApiException("Network error occurred", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Request timeout for {Endpoint}", endpoint);
            throw new FinnhubApiException("Request timed out", ex);
        }
    }

    /// <summary>
    /// Processes the HTTP response and deserializes the content
    /// </summary>
    /// <typeparam name="T">Response type</typeparam>
    /// <param name="response">HTTP response message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized response</returns>
    private async Task<T> ProcessResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response, content);
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions) ?? throw new FinnhubApiException(
                    "API returned null response",
                    response.StatusCode,
                    content);
            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response: {Content}", content);
            throw new FinnhubApiException(
                "Invalid JSON response from API",
                response.StatusCode,
                content);
        }
    }

    /// <summary>
    /// Handles error responses from the API
    /// </summary>
    /// <param name="response">HTTP response</param>
    /// <param name="content">Response content</param>
    private async Task HandleErrorResponseAsync(HttpResponseMessage response, string content)
    {
        _logger.LogError(
            "API returned error {StatusCode}: {Content}",
            response.StatusCode,
            content);

        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                throw new FinnhubAuthenticationException(
                    "Invalid API key or unauthorized access");

            case HttpStatusCode.TooManyRequests:
                // RateLimitHandler should have already thrown, but handle it here as fallback
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromMinutes(1);
                throw new FinnhubRateLimitException(
                    "Rate limit exceeded",
                    DateTime.UtcNow.Add(retryAfter));

            case HttpStatusCode.NotFound:
                throw new FinnhubApiException(
                    "Resource not found",
                    response.StatusCode,
                    content);

            case HttpStatusCode.BadRequest:
                throw new FinnhubApiException(
                    "Bad request - invalid parameters",
                    response.StatusCode,
                    content);

            default:
                throw new FinnhubApiException(
                    $"API request failed with status {response.StatusCode}",
                    response.StatusCode,
                    content);
        }
    }

    /// <summary>
    /// Builds a complete URL with query parameters
    /// </summary>
    /// <param name="endpoint">API endpoint</param>
    /// <param name="queryParameters">Query parameters</param>
    /// <returns>Complete URL</returns>
    private static string BuildUrl(string endpoint, Dictionary<string, string>? queryParameters)
    {
        if (queryParameters == null || queryParameters.Count == 0)
        {
            return endpoint;
        }

        var queryString = string.Join("&",
            queryParameters.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        return $"{endpoint}?{queryString}";
    }
}
