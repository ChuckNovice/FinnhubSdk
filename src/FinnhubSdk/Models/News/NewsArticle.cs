namespace FinnhubSdk.Models.News;

/// <summary>
/// News article from company or market news
/// </summary>
public sealed class NewsArticle
{
    /// <summary>
    /// News article ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// News category (e.g., "company", "general", "forex", "crypto", "merger")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Published time in UNIX timestamp
    /// </summary>
    public long DateTime { get; set; }

    /// <summary>
    /// News headline
    /// </summary>
    public string Headline { get; set; } = string.Empty;

    /// <summary>
    /// Original image URL
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Related symbols
    /// </summary>
    public string Related { get; set; } = string.Empty;

    /// <summary>
    /// News source
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// News summary
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// URL to the original article
    /// </summary>
    public string Url { get; set; } = string.Empty;
}
