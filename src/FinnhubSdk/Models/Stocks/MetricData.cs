using System.Text.Json.Serialization;

namespace FinnhubSdk.Models.Stocks;

/// <summary>
/// Financial metric values for a company
/// </summary>
public sealed class MetricData
{
    /// <summary>
    /// 52-week high price
    /// </summary>
    [JsonPropertyName("52WeekHigh")]
    public decimal? FiftyTwoWeekHigh { get; set; }

    /// <summary>
    /// 52-week high date
    /// </summary>
    [JsonPropertyName("52WeekHighDate")]
    public string? FiftyTwoWeekHighDate { get; set; }

    /// <summary>
    /// 52-week low price
    /// </summary>
    [JsonPropertyName("52WeekLow")]
    public decimal? FiftyTwoWeekLow { get; set; }

    /// <summary>
    /// 52-week low date
    /// </summary>
    [JsonPropertyName("52WeekLowDate")]
    public string? FiftyTwoWeekLowDate { get; set; }

    /// <summary>
    /// 52-week price return daily
    /// </summary>
    [JsonPropertyName("52WeekPriceReturnDaily")]
    public decimal? FiftyTwoWeekPriceReturnDaily { get; set; }

    /// <summary>
    /// 10-day average trading volume
    /// </summary>
    [JsonPropertyName("10DayAverageTradingVolume")]
    public decimal? TenDayAverageTradingVolume { get; set; }

    /// <summary>
    /// 3-month average trading volume
    /// </summary>
    [JsonPropertyName("3MonthAverageTradingVolume")]
    public decimal? ThreeMonthAverageTradingVolume { get; set; }

    /// <summary>
    /// Beta value
    /// </summary>
    public decimal? Beta { get; set; }

    /// <summary>
    /// Book value per share (annual)
    /// </summary>
    public decimal? BookValuePerShareAnnual { get; set; }

    /// <summary>
    /// Book value per share (quarterly)
    /// </summary>
    public decimal? BookValuePerShareQuarterly { get; set; }

    /// <summary>
    /// Current dividend yield (TTM)
    /// </summary>
    public decimal? CurrentDividendYieldTTM { get; set; }

    /// <summary>
    /// Current ratio (annual)
    /// </summary>
    public decimal? CurrentRatioAnnual { get; set; }

    /// <summary>
    /// Current ratio (quarterly)
    /// </summary>
    public decimal? CurrentRatioQuarterly { get; set; }

    /// <summary>
    /// Dividend per share (annual)
    /// </summary>
    public decimal? DividendPerShareAnnual { get; set; }

    /// <summary>
    /// Dividend yield indicated (annual)
    /// </summary>
    public decimal? DividendYieldIndicatedAnnual { get; set; }

    /// <summary>
    /// EPS basic excluding extraordinary items (annual)
    /// </summary>
    public decimal? EpsBasicExclExtraItemsAnnual { get; set; }

    /// <summary>
    /// EPS basic excluding extraordinary items (TTM)
    /// </summary>
    public decimal? EpsBasicExclExtraItemsTTM { get; set; }

    /// <summary>
    /// EPS growth (TTM vs TTM)
    /// </summary>
    public decimal? EpsGrowthTTMYoy { get; set; }

    /// <summary>
    /// EPS excluding extraordinary items (annual)
    /// </summary>
    public decimal? EpsExclExtraItemsAnnual { get; set; }

    /// <summary>
    /// EPS excluding extraordinary items (TTM)
    /// </summary>
    public decimal? EpsExclExtraItemsTTM { get; set; }

    /// <summary>
    /// EPS including extraordinary items (annual)
    /// </summary>
    public decimal? EpsInclExtraItemsAnnual { get; set; }

    /// <summary>
    /// EPS including extraordinary items (TTM)
    /// </summary>
    public decimal? EpsInclExtraItemsTTM { get; set; }

    /// <summary>
    /// EPS normalized (annual)
    /// </summary>
    public decimal? EpsNormalizedAnnual { get; set; }

    /// <summary>
    /// Free cash flow per share (TTM)
    /// </summary>
    public decimal? FreeCashFlowPerShareTTM { get; set; }

    /// <summary>
    /// Gross margin (5-year average)
    /// </summary>
    public decimal? GrossMargin5Y { get; set; }

    /// <summary>
    /// Gross margin (annual)
    /// </summary>
    public decimal? GrossMarginAnnual { get; set; }

    /// <summary>
    /// Gross margin (TTM)
    /// </summary>
    public decimal? GrossMarginTTM { get; set; }

    /// <summary>
    /// Market capitalization
    /// </summary>
    public decimal? MarketCapitalization { get; set; }

    /// <summary>
    /// Net debt (annual)
    /// </summary>
    public decimal? NetDebtAnnual { get; set; }

    /// <summary>
    /// Net debt (quarterly)
    /// </summary>
    public decimal? NetDebtQuarterly { get; set; }

    /// <summary>
    /// Net interest coverage (annual)
    /// </summary>
    public decimal? NetInterestCoverageAnnual { get; set; }

    /// <summary>
    /// Net interest coverage (TTM)
    /// </summary>
    public decimal? NetInterestCoverageTTM { get; set; }

    /// <summary>
    /// Net margin growth (5-year)
    /// </summary>
    public decimal? NetMarginGrowth5Y { get; set; }

    /// <summary>
    /// Net profit margin (5-year average)
    /// </summary>
    public decimal? NetProfitMargin5Y { get; set; }

    /// <summary>
    /// Net profit margin (annual)
    /// </summary>
    public decimal? NetProfitMarginAnnual { get; set; }

    /// <summary>
    /// Net profit margin (TTM)
    /// </summary>
    public decimal? NetProfitMarginTTM { get; set; }

    /// <summary>
    /// Operating margin (5-year average)
    /// </summary>
    public decimal? OperatingMargin5Y { get; set; }

    /// <summary>
    /// Operating margin (annual)
    /// </summary>
    public decimal? OperatingMarginAnnual { get; set; }

    /// <summary>
    /// Operating margin (TTM)
    /// </summary>
    public decimal? OperatingMarginTTM { get; set; }

    /// <summary>
    /// Payout ratio (annual)
    /// </summary>
    public decimal? PayoutRatioAnnual { get; set; }

    /// <summary>
    /// Payout ratio (TTM)
    /// </summary>
    public decimal? PayoutRatioTTM { get; set; }

    /// <summary>
    /// P/E basic excluding extraordinary items (annual)
    /// </summary>
    public decimal? PeBasicExclExtraAnnual { get; set; }

    /// <summary>
    /// P/E basic excluding extraordinary items (TTM)
    /// </summary>
    public decimal? PeBasicExclExtraTTM { get; set; }

    /// <summary>
    /// P/E excluding extraordinary items (annual)
    /// </summary>
    public decimal? PeExclExtraAnnual { get; set; }

    /// <summary>
    /// P/E excluding extraordinary items (TTM)
    /// </summary>
    public decimal? PeExclExtraTTM { get; set; }

    /// <summary>
    /// P/E including extraordinary items (annual)
    /// </summary>
    public decimal? PeInclExtraAnnual { get; set; }

    /// <summary>
    /// P/E including extraordinary items (TTM)
    /// </summary>
    public decimal? PeInclExtraTTM { get; set; }

    /// <summary>
    /// P/E normalized (annual)
    /// </summary>
    public decimal? PeNormalizedAnnual { get; set; }

    /// <summary>
    /// Price to book (annual)
    /// </summary>
    public decimal? PbAnnual { get; set; }

    /// <summary>
    /// Price to book (quarterly)
    /// </summary>
    public decimal? PbQuarterly { get; set; }

    /// <summary>
    /// Price to cash flow per share (TTM)
    /// </summary>
    public decimal? PfcfShareTTM { get; set; }

    /// <summary>
    /// Price to sales (annual)
    /// </summary>
    public decimal? PsAnnual { get; set; }

    /// <summary>
    /// Price to sales (TTM)
    /// </summary>
    public decimal? PsTTM { get; set; }

    /// <summary>
    /// Price to tangible book (annual)
    /// </summary>
    public decimal? PtbvAnnual { get; set; }

    /// <summary>
    /// Price to tangible book (quarterly)
    /// </summary>
    public decimal? PtbvQuarterly { get; set; }

    /// <summary>
    /// Quick ratio (annual)
    /// </summary>
    public decimal? QuickRatioAnnual { get; set; }

    /// <summary>
    /// Quick ratio (quarterly)
    /// </summary>
    public decimal? QuickRatioQuarterly { get; set; }

    /// <summary>
    /// Return on assets (ROA - 5-year average)
    /// </summary>
    public decimal? RoaRfy { get; set; }

    /// <summary>
    /// Return on assets (ROA - TTM)
    /// </summary>
    public decimal? RoaTTM { get; set; }

    /// <summary>
    /// Return on equity (ROE - 5-year average)
    /// </summary>
    public decimal? RoeRfy { get; set; }

    /// <summary>
    /// Return on equity (ROE - TTM)
    /// </summary>
    public decimal? RoeTTM { get; set; }

    /// <summary>
    /// Return on invested capital (ROIC - annual)
    /// </summary>
    public decimal? RoiAnnual { get; set; }

    /// <summary>
    /// Return on invested capital (ROIC - TTM)
    /// </summary>
    public decimal? RoiTTM { get; set; }

    /// <summary>
    /// Revenue per share (annual)
    /// </summary>
    public decimal? RevenuePerShareAnnual { get; set; }

    /// <summary>
    /// Revenue per share (TTM)
    /// </summary>
    public decimal? RevenuePerShareTTM { get; set; }

    /// <summary>
    /// Revenue growth (3-year)
    /// </summary>
    public decimal? RevenueGrowth3Y { get; set; }

    /// <summary>
    /// Revenue growth (5-year)
    /// </summary>
    public decimal? RevenueGrowth5Y { get; set; }

    /// <summary>
    /// Revenue growth (quarterly YoY)
    /// </summary>
    public decimal? RevenueGrowthQuarterlyYoy { get; set; }

    /// <summary>
    /// Revenue growth (TTM YoY)
    /// </summary>
    public decimal? RevenueGrowthTTMYoy { get; set; }

    /// <summary>
    /// Tangible book value per share (annual)
    /// </summary>
    public decimal? TangibleBookValuePerShareAnnual { get; set; }

    /// <summary>
    /// Tangible book value per share (quarterly)
    /// </summary>
    public decimal? TangibleBookValuePerShareQuarterly { get; set; }

    /// <summary>
    /// Total debt to equity (annual)
    /// </summary>
    public decimal? TotalDebtToEquityAnnual { get; set; }

    /// <summary>
    /// Total debt to equity (quarterly)
    /// </summary>
    public decimal? TotalDebtToEquityQuarterly { get; set; }

    /// <summary>
    /// Total debt to total assets (annual)
    /// </summary>
    public decimal? TotalDebtToTotalAssetsAnnual { get; set; }

    /// <summary>
    /// Total debt to total assets (quarterly)
    /// </summary>
    public decimal? TotalDebtToTotalAssetsQuarterly { get; set; }

    /// <summary>
    /// Total debt to total capital (annual)
    /// </summary>
    public decimal? TotalDebtToTotalCapitalAnnual { get; set; }

    /// <summary>
    /// Total debt to total capital (quarterly)
    /// </summary>
    public decimal? TotalDebtToTotalCapitalQuarterly { get; set; }
}
