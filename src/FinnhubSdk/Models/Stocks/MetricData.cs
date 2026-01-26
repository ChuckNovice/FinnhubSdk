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
    [JsonPropertyName("beta")]
    public decimal? Beta { get; set; }

    /// <summary>
    /// Book value per share (annual)
    /// </summary>
    [JsonPropertyName("bookValuePerShareAnnual")]
    public decimal? BookValuePerShareAnnual { get; set; }

    /// <summary>
    /// Book value per share (quarterly)
    /// </summary>
    [JsonPropertyName("bookValuePerShareQuarterly")]
    public decimal? BookValuePerShareQuarterly { get; set; }

    /// <summary>
    /// Current dividend yield (TTM)
    /// </summary>
    [JsonPropertyName("currentDividendYieldTTM")]
    public decimal? CurrentDividendYieldTTM { get; set; }

    /// <summary>
    /// Current ratio (annual)
    /// </summary>
    [JsonPropertyName("currentRatioAnnual")]
    public decimal? CurrentRatioAnnual { get; set; }

    /// <summary>
    /// Current ratio (quarterly)
    /// </summary>
    [JsonPropertyName("currentRatioQuarterly")]
    public decimal? CurrentRatioQuarterly { get; set; }

    /// <summary>
    /// Dividend per share (annual)
    /// </summary>
    [JsonPropertyName("dividendPerShareAnnual")]
    public decimal? DividendPerShareAnnual { get; set; }

    /// <summary>
    /// Dividend yield indicated (annual)
    /// </summary>
    [JsonPropertyName("dividendYieldIndicatedAnnual")]
    public decimal? DividendYieldIndicatedAnnual { get; set; }

    /// <summary>
    /// EPS basic excluding extraordinary items (annual)
    /// </summary>
    [JsonPropertyName("epsBasicExclExtraItemsAnnual")]
    public decimal? EpsBasicExclExtraItemsAnnual { get; set; }

    /// <summary>
    /// EPS basic excluding extraordinary items (TTM)
    /// </summary>
    [JsonPropertyName("epsBasicExclExtraItemsTTM")]
    public decimal? EpsBasicExclExtraItemsTTM { get; set; }

    /// <summary>
    /// EPS growth (TTM vs TTM)
    /// </summary>
    [JsonPropertyName("epsGrowthTTMYoy")]
    public decimal? EpsGrowthTTMYoy { get; set; }

    /// <summary>
    /// EPS excluding extraordinary items (annual)
    /// </summary>
    [JsonPropertyName("epsExclExtraItemsAnnual")]
    public decimal? EpsExclExtraItemsAnnual { get; set; }

    /// <summary>
    /// EPS excluding extraordinary items (TTM)
    /// </summary>
    [JsonPropertyName("epsExclExtraItemsTTM")]
    public decimal? EpsExclExtraItemsTTM { get; set; }

    /// <summary>
    /// EPS including extraordinary items (annual)
    /// </summary>
    [JsonPropertyName("epsInclExtraItemsAnnual")]
    public decimal? EpsInclExtraItemsAnnual { get; set; }

    /// <summary>
    /// EPS including extraordinary items (TTM)
    /// </summary>
    [JsonPropertyName("epsInclExtraItemsTTM")]
    public decimal? EpsInclExtraItemsTTM { get; set; }

    /// <summary>
    /// EPS normalized (annual)
    /// </summary>
    [JsonPropertyName("epsNormalizedAnnual")]
    public decimal? EpsNormalizedAnnual { get; set; }

    /// <summary>
    /// Free cash flow per share (TTM)
    /// </summary>
    [JsonPropertyName("freeCashFlowPerShareTTM")]
    public decimal? FreeCashFlowPerShareTTM { get; set; }

    /// <summary>
    /// Gross margin (5-year average)
    /// </summary>
    [JsonPropertyName("grossMargin5Y")]
    public decimal? GrossMargin5Y { get; set; }

    /// <summary>
    /// Gross margin (annual)
    /// </summary>
    [JsonPropertyName("grossMarginAnnual")]
    public decimal? GrossMarginAnnual { get; set; }

    /// <summary>
    /// Gross margin (TTM)
    /// </summary>
    [JsonPropertyName("grossMarginTTM")]
    public decimal? GrossMarginTTM { get; set; }

    /// <summary>
    /// Market capitalization
    /// </summary>
    [JsonPropertyName("marketCapitalization")]
    public decimal? MarketCapitalization { get; set; }

    /// <summary>
    /// Net debt (annual)
    /// </summary>
    [JsonPropertyName("netDebtAnnual")]
    public decimal? NetDebtAnnual { get; set; }

    /// <summary>
    /// Net debt (quarterly)
    /// </summary>
    [JsonPropertyName("netDebtQuarterly")]
    public decimal? NetDebtQuarterly { get; set; }

    /// <summary>
    /// Net interest coverage (annual)
    /// </summary>
    [JsonPropertyName("netInterestCoverageAnnual")]
    public decimal? NetInterestCoverageAnnual { get; set; }

    /// <summary>
    /// Net interest coverage (TTM)
    /// </summary>
    [JsonPropertyName("netInterestCoverageTTM")]
    public decimal? NetInterestCoverageTTM { get; set; }

    /// <summary>
    /// Net margin growth (5-year)
    /// </summary>
    [JsonPropertyName("netMarginGrowth5Y")]
    public decimal? NetMarginGrowth5Y { get; set; }

    /// <summary>
    /// Net profit margin (5-year average)
    /// </summary>
    [JsonPropertyName("netProfitMargin5Y")]
    public decimal? NetProfitMargin5Y { get; set; }

    /// <summary>
    /// Net profit margin (annual)
    /// </summary>
    [JsonPropertyName("netProfitMarginAnnual")]
    public decimal? NetProfitMarginAnnual { get; set; }

    /// <summary>
    /// Net profit margin (TTM)
    /// </summary>
    [JsonPropertyName("netProfitMarginTTM")]
    public decimal? NetProfitMarginTTM { get; set; }

    /// <summary>
    /// Operating margin (5-year average)
    /// </summary>
    [JsonPropertyName("operatingMargin5Y")]
    public decimal? OperatingMargin5Y { get; set; }

    /// <summary>
    /// Operating margin (annual)
    /// </summary>
    [JsonPropertyName("operatingMarginAnnual")]
    public decimal? OperatingMarginAnnual { get; set; }

    /// <summary>
    /// Operating margin (TTM)
    /// </summary>
    [JsonPropertyName("operatingMarginTTM")]
    public decimal? OperatingMarginTTM { get; set; }

    /// <summary>
    /// Payout ratio (annual)
    /// </summary>
    [JsonPropertyName("payoutRatioAnnual")]
    public decimal? PayoutRatioAnnual { get; set; }

    /// <summary>
    /// Payout ratio (TTM)
    /// </summary>
    [JsonPropertyName("payoutRatioTTM")]
    public decimal? PayoutRatioTTM { get; set; }

    /// <summary>
    /// P/E basic excluding extraordinary items (annual)
    /// </summary>
    [JsonPropertyName("peBasicExclExtraAnnual")]
    public decimal? PeBasicExclExtraAnnual { get; set; }

    /// <summary>
    /// P/E basic excluding extraordinary items (TTM)
    /// </summary>
    [JsonPropertyName("peBasicExclExtraTTM")]
    public decimal? PeBasicExclExtraTTM { get; set; }

    /// <summary>
    /// P/E excluding extraordinary items (annual)
    /// </summary>
    [JsonPropertyName("peExclExtraAnnual")]
    public decimal? PeExclExtraAnnual { get; set; }

    /// <summary>
    /// P/E excluding extraordinary items (TTM)
    /// </summary>
    [JsonPropertyName("peExclExtraTTM")]
    public decimal? PeExclExtraTTM { get; set; }

    /// <summary>
    /// P/E including extraordinary items (annual)
    /// </summary>
    [JsonPropertyName("peInclExtraAnnual")]
    public decimal? PeInclExtraAnnual { get; set; }

    /// <summary>
    /// P/E including extraordinary items (TTM)
    /// </summary>
    [JsonPropertyName("peInclExtraTTM")]
    public decimal? PeInclExtraTTM { get; set; }

    /// <summary>
    /// P/E normalized (annual)
    /// </summary>
    [JsonPropertyName("peNormalizedAnnual")]
    public decimal? PeNormalizedAnnual { get; set; }

    /// <summary>
    /// Price to book (annual)
    /// </summary>
    [JsonPropertyName("pbAnnual")]
    public decimal? PbAnnual { get; set; }

    /// <summary>
    /// Price to book (quarterly)
    /// </summary>
    [JsonPropertyName("pbQuarterly")]
    public decimal? PbQuarterly { get; set; }

    /// <summary>
    /// Price to cash flow per share (TTM)
    /// </summary>
    [JsonPropertyName("pfcfShareTTM")]
    public decimal? PfcfShareTTM { get; set; }

    /// <summary>
    /// Price to sales (annual)
    /// </summary>
    [JsonPropertyName("psAnnual")]
    public decimal? PsAnnual { get; set; }

    /// <summary>
    /// Price to sales (TTM)
    /// </summary>
    [JsonPropertyName("psTTM")]
    public decimal? PsTTM { get; set; }

    /// <summary>
    /// Price to tangible book (annual)
    /// </summary>
    [JsonPropertyName("ptbvAnnual")]
    public decimal? PtbvAnnual { get; set; }

    /// <summary>
    /// Price to tangible book (quarterly)
    /// </summary>
    [JsonPropertyName("ptbvQuarterly")]
    public decimal? PtbvQuarterly { get; set; }

    /// <summary>
    /// Quick ratio (annual)
    /// </summary>
    [JsonPropertyName("quickRatioAnnual")]
    public decimal? QuickRatioAnnual { get; set; }

    /// <summary>
    /// Quick ratio (quarterly)
    /// </summary>
    [JsonPropertyName("quickRatioQuarterly")]
    public decimal? QuickRatioQuarterly { get; set; }

    /// <summary>
    /// Return on assets (ROA - 5-year average)
    /// </summary>
    [JsonPropertyName("roaRfy")]
    public decimal? RoaRfy { get; set; }

    /// <summary>
    /// Return on assets (ROA - TTM)
    /// </summary>
    [JsonPropertyName("roaTTM")]
    public decimal? RoaTTM { get; set; }

    /// <summary>
    /// Return on equity (ROE - 5-year average)
    /// </summary>
    [JsonPropertyName("roeRfy")]
    public decimal? RoeRfy { get; set; }

    /// <summary>
    /// Return on equity (ROE - TTM)
    /// </summary>
    [JsonPropertyName("roeTTM")]
    public decimal? RoeTTM { get; set; }

    /// <summary>
    /// Return on invested capital (ROIC - annual)
    /// </summary>
    [JsonPropertyName("roiAnnual")]
    public decimal? RoiAnnual { get; set; }

    /// <summary>
    /// Return on invested capital (ROIC - TTM)
    /// </summary>
    [JsonPropertyName("roiTTM")]
    public decimal? RoiTTM { get; set; }

    /// <summary>
    /// Revenue per share (annual)
    /// </summary>
    [JsonPropertyName("revenuePerShareAnnual")]
    public decimal? RevenuePerShareAnnual { get; set; }

    /// <summary>
    /// Revenue per share (TTM)
    /// </summary>
    [JsonPropertyName("revenuePerShareTTM")]
    public decimal? RevenuePerShareTTM { get; set; }

    /// <summary>
    /// Revenue growth (3-year)
    /// </summary>
    [JsonPropertyName("revenueGrowth3Y")]
    public decimal? RevenueGrowth3Y { get; set; }

    /// <summary>
    /// Revenue growth (5-year)
    /// </summary>
    [JsonPropertyName("revenueGrowth5Y")]
    public decimal? RevenueGrowth5Y { get; set; }

    /// <summary>
    /// Revenue growth (quarterly YoY)
    /// </summary>
    [JsonPropertyName("revenueGrowthQuarterlyYoy")]
    public decimal? RevenueGrowthQuarterlyYoy { get; set; }

    /// <summary>
    /// Revenue growth (TTM YoY)
    /// </summary>
    [JsonPropertyName("revenueGrowthTTMYoy")]
    public decimal? RevenueGrowthTTMYoy { get; set; }

    /// <summary>
    /// Tangible book value per share (annual)
    /// </summary>
    [JsonPropertyName("tangibleBookValuePerShareAnnual")]
    public decimal? TangibleBookValuePerShareAnnual { get; set; }

    /// <summary>
    /// Tangible book value per share (quarterly)
    /// </summary>
    [JsonPropertyName("tangibleBookValuePerShareQuarterly")]
    public decimal? TangibleBookValuePerShareQuarterly { get; set; }

    /// <summary>
    /// Total debt to equity (annual)
    /// </summary>
    [JsonPropertyName("totalDebtToEquityAnnual")]
    public decimal? TotalDebtToEquityAnnual { get; set; }

    /// <summary>
    /// Total debt to equity (quarterly)
    /// </summary>
    [JsonPropertyName("totalDebtToEquityQuarterly")]
    public decimal? TotalDebtToEquityQuarterly { get; set; }

    /// <summary>
    /// Total debt to total assets (annual)
    /// </summary>
    [JsonPropertyName("totalDebtToTotalAssetsAnnual")]
    public decimal? TotalDebtToTotalAssetsAnnual { get; set; }

    /// <summary>
    /// Total debt to total assets (quarterly)
    /// </summary>
    [JsonPropertyName("totalDebtToTotalAssetsQuarterly")]
    public decimal? TotalDebtToTotalAssetsQuarterly { get; set; }

    /// <summary>
    /// Total debt to total capital (annual)
    /// </summary>
    [JsonPropertyName("totalDebtToTotalCapitalAnnual")]
    public decimal? TotalDebtToTotalCapitalAnnual { get; set; }

    /// <summary>
    /// Total debt to total capital (quarterly)
    /// </summary>
    [JsonPropertyName("totalDebtToTotalCapitalQuarterly")]
    public decimal? TotalDebtToTotalCapitalQuarterly { get; set; }
}
