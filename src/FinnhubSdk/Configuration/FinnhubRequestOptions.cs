namespace FinnhubSdk.Configuration;

/// <summary>
/// HTTP request options keys for passing per-request authentication data.
/// Used internally by factory-created clients to pass API key through the HTTP pipeline.
/// </summary>
internal static class FinnhubRequestOptions
{
    /// <summary>
    /// Key for storing the API key in HttpRequestMessage.Options
    /// </summary>
    public static readonly HttpRequestOptionsKey<string> ApiKey = new("Finnhub-ApiKey");

    /// <summary>
    /// Key for storing the authentication method in HttpRequestMessage.Options
    /// </summary>
    public static readonly HttpRequestOptionsKey<AuthenticationMethod> AuthMethod = new("Finnhub-AuthMethod");
}
