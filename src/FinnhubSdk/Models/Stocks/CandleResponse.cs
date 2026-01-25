using System.Text.Json.Serialization;

namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Internal response model for candle data from Finnhub API
/// The API returns parallel arrays for each field
/// </summary>
internal sealed class CandleResponse
{
    /// <summary>
    /// List of close prices
    /// </summary>
    [JsonPropertyName("c")]
    public decimal[] Close { get; set; } = Array.Empty<decimal>();

    /// <summary>
    /// List of high prices
    /// </summary>
    [JsonPropertyName("h")]
    public decimal[] High { get; set; } = Array.Empty<decimal>();

    /// <summary>
    /// List of low prices
    /// </summary>
    [JsonPropertyName("l")]
    public decimal[] Low { get; set; } = Array.Empty<decimal>();

    /// <summary>
    /// List of open prices
    /// </summary>
    [JsonPropertyName("o")]
    public decimal[] Open { get; set; } = Array.Empty<decimal>();

    /// <summary>
    /// List of volumes
    /// </summary>
    [JsonPropertyName("v")]
    public long[] Volume { get; set; } = Array.Empty<long>();

    /// <summary>
    /// List of Unix timestamps
    /// </summary>
    [JsonPropertyName("t")]
    public long[] Timestamp { get; set; } = Array.Empty<long>();

    /// <summary>
    /// Status of the request (ok or no_data)
    /// </summary>
    [JsonPropertyName("s")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Converts the response to a list of Candle objects
    /// </summary>
    /// <returns>List of candles</returns>
    public IReadOnlyList<Candle> ToCandles()
    {
        if (Status != "ok" || Close.Length == 0)
        {
            return Array.Empty<Candle>();
        }

        var candles = new List<Candle>(Close.Length);
        for (int i = 0; i < Close.Length; i++)
        {
            candles.Add(new Candle
            {
                Open = Open[i],
                High = High[i],
                Low = Low[i],
                Close = Close[i],
                Volume = Volume[i],
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(Timestamp[i]).UtcDateTime
            });
        }

        return candles;
    }
}
