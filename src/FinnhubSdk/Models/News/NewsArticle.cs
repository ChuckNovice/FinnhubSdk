using System.Text.Json.Serialization;

namespace FinnhubSdk.Models.News;

/// <summary>
/// News article from company or market news
/// </summary>
public sealed class NewsArticle
{
    /// <summary>
    /// News article ID
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// News category (e.g., "company", "general", "forex", "crypto", "merger")
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Published time in UNIX timestamp
    /// </summary>
    [JsonPropertyName("datetime")]
    public long DateTime { get; set; }

    /// <summary>
    /// News headline
    /// </summary>
    [JsonPropertyName("headline")]
    public string Headline { get; set; } = string.Empty;

    /// <summary>
    /// Original image URL
    /// </summary>
    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Related symbols
    /// </summary>
    [JsonPropertyName("related")]
    public string Related { get; set; } = string.Empty;

    /// <summary>
    /// News source
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// News summary
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// URL to the original article
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}
