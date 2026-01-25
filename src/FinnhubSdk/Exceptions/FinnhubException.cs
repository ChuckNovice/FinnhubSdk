using System.Net;

namespace FinnhubSdk.Exceptions;

/// <summary>
/// Base exception for all Finnhub SDK errors
/// </summary>
public abstract class FinnhubException : Exception
{
    /// <summary>
    /// Gets the request ID if available
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets the HTTP status code if applicable
    /// </summary>
    public HttpStatusCode? StatusCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubException"/> class
    /// </summary>
    /// <param name="message">The error message</param>
    protected FinnhubException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubException"/> class
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    protected FinnhubException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
