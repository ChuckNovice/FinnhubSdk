using System.Net;

namespace FinnhubSdk.Exceptions;

/// <summary>
/// Exception thrown when the Finnhub API returns an error response
/// </summary>
public class FinnhubApiException : FinnhubException
{
    /// <summary>
    /// Gets the error code from the API response if available
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets the raw response body if available
    /// </summary>
    public object? ResponseBody { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubApiException"/> class
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="responseContent">The response content</param>
    public FinnhubApiException(
        string message,
        HttpStatusCode? statusCode = null,
        string? responseContent = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseContent;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubApiException"/> class
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public FinnhubApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
