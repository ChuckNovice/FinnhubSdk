namespace FinnhubSdk.WebSocket.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a trade message received from the Finnhub WebSocket
/// </summary>
public sealed class TradeMessage
{
    /// <summary>
    /// Message type (typically "trade" for trade data)
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// List of trades in this message
    /// </summary>
    [JsonPropertyName("data")]
    public Trade[] Data { get; set; } = [];
}
