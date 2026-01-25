using System.Text.Json.Serialization;

namespace FinnhubSdk.WebSocket.Models;

/// <summary>
/// Represents a single trade from the real-time WebSocket feed
/// </summary>
public sealed class Trade
{
    /// <summary>
    /// Symbol of the trade
    /// </summary>
    [JsonPropertyName("s")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Last price
    /// </summary>
    [JsonPropertyName("p")]
    public decimal Price { get; set; }

    /// <summary>
    /// Volume
    /// </summary>
    [JsonPropertyName("v")]
    public decimal Volume { get; set; }

    /// <summary>
    /// UNIX milliseconds timestamp
    /// </summary>
    [JsonPropertyName("t")]
    public long Timestamp { get; set; }

    /// <summary>
    /// List of trade conditions (if applicable)
    /// </summary>
    [JsonPropertyName("c")]
    public string[]? Conditions { get; set; }
}
