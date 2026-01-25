namespace FinnhubSdk.Exceptions;

/// <summary>
/// Exception thrown when authentication with the Finnhub API fails (HTTP 401)
/// </summary>
public class FinnhubAuthenticationException : FinnhubException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubAuthenticationException"/> class
    /// </summary>
    /// <param name="message">The error message</param>
    public FinnhubAuthenticationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubAuthenticationException"/> class
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public FinnhubAuthenticationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
