namespace FinnhubSdk.WebSocket;

/// <summary>
/// Represents the connection state of the WebSocket client
/// </summary>
public enum ConnectionState
{
    /// <summary>
    /// Not connected to the WebSocket server
    /// </summary>
    Disconnected,

    /// <summary>
    /// Currently establishing connection to the WebSocket server
    /// </summary>
    Connecting,

    /// <summary>
    /// Successfully connected to the WebSocket server
    /// </summary>
    Connected,

    /// <summary>
    /// Connection lost, attempting to reconnect
    /// </summary>
    Reconnecting,

    /// <summary>
    /// Connection failed after maximum reconnection attempts
    /// </summary>
    Failed
}
