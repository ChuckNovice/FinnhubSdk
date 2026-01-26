namespace FinnhubSdk.Models.News;

/// <summary>
/// News sentiment analysis for a company
/// </summary>
public sealed class NewsSentiment
{
    /// <summary>
    /// Symbol of the company
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Buzz statistics
    /// </summary>
    public SentimentBuzz? Buzz { get; set; }

    /// <summary>
    /// Company news score
    /// </summary>
    public decimal CompanyNewsScore { get; set; }

    /// <summary>
    /// Sector average bullish percent
    /// </summary>
    public decimal SectorAverageBullishPercent { get; set; }

    /// <summary>
    /// Sector average news score
    /// </summary>
    public decimal SectorAverageNewsScore { get; set; }

    /// <summary>
    /// Sentiment analysis data
    /// </summary>
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
    public int ArticlesInLastWeek { get; set; }

    /// <summary>
    /// Buzz score
    /// </summary>
    public decimal Buzz { get; set; }

    /// <summary>
    /// Weekly average buzz
    /// </summary>
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
    public decimal BearishPercent { get; set; }

    /// <summary>
    /// Bullish percent (positive sentiment)
    /// </summary>
    public decimal BullishPercent { get; set; }
}
