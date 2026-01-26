namespace FinnhubSdk.WebSocket.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a message to send to the Finnhub WebSocket (subscribe/unsubscribe)
/// </summary>
internal sealed class WebSocketMessage
{
    /// <summary>
    /// Message type ("subscribe" or "unsubscribe")
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Symbol to subscribe/unsubscribe
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Creates a subscribe message for the given symbol
    /// </summary>
    public static WebSocketMessage Subscribe(string symbol) => new()
    {
        Type = "subscribe",
        Symbol = symbol
    };

    /// <summary>
    /// Creates an unsubscribe message for the given symbol
    /// </summary>
    public static WebSocketMessage Unsubscribe(string symbol) => new()
    {
        Type = "unsubscribe",
        Symbol = symbol
    };
}
