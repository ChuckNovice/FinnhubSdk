namespace FinnhubSdk.Exceptions;

using System.Net.WebSockets;

/// <summary>
/// Exception thrown when WebSocket operations fail
/// </summary>
public class FinnhubWebSocketException : FinnhubException
{
    /// <summary>
    /// Gets the WebSocket error type if available
    /// </summary>
    public WebSocketError? WebSocketError { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubWebSocketException"/> class
    /// </summary>
    /// <param name="message">The error message</param>
    public FinnhubWebSocketException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubWebSocketException"/> class
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public FinnhubWebSocketException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
