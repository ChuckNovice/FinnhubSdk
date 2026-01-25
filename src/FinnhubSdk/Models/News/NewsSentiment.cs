using System.Text.Json.Serialization;

namespace FinnhubSdk.Models.News;

/// <summary>
/// News sentiment analysis for a company
/// </summary>
public sealed class NewsSentiment
{
    /// <summary>
    /// Symbol of the company
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Buzz statistics
    /// </summary>
    [JsonPropertyName("buzz")]
    public SentimentBuzz? Buzz { get; set; }

    /// <summary>
    /// Company news score
    /// </summary>
    [JsonPropertyName("companyNewsScore")]
    public decimal CompanyNewsScore { get; set; }

    /// <summary>
    /// Sector average bullish percent
    /// </summary>
    [JsonPropertyName("sectorAverageBullishPercent")]
    public decimal SectorAverageBullishPercent { get; set; }

    /// <summary>
    /// Sector average news score
    /// </summary>
    [JsonPropertyName("sectorAverageNewsScore")]
    public decimal SectorAverageNewsScore { get; set; }

    /// <summary>
    /// Sentiment analysis data
    /// </summary>
    [JsonPropertyName("sentiment")]
    public SentimentData? Sentiment { get; set; }
}

/// <summary>
/// Buzz statistics for news sentiment
/// </summary>
public sealed class SentimentBuzz
{
    /// <summary>
    /// Number of articles in the last week
    /// </summary>
    [JsonPropertyName("articlesInLastWeek")]
    public int ArticlesInLastWeek { get; set; }

    /// <summary>
    /// Buzz score
    /// </summary>
    [JsonPropertyName("buzz")]
    public decimal Buzz { get; set; }

    /// <summary>
    /// Weekly average buzz
    /// </summary>
    [JsonPropertyName("weeklyAverage")]
    public decimal WeeklyAverage { get; set; }
}

/// <summary>
/// Sentiment analysis data
/// </summary>
public sealed class SentimentData
{
    /// <summary>
    /// Bearish percent (negative sentiment)
    /// </summary>
    [JsonPropertyName("bearishPercent")]
    public decimal BearishPercent { get; set; }

    /// <summary>
    /// Bullish percent (positive sentiment)
    /// </summary>
    [JsonPropertyName("bullishPercent")]
    public decimal BullishPercent { get; set; }
}
