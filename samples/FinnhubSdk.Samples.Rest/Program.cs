using FinnhubSdk.Clients;
using FinnhubSdk.Extensions;
using FinnhubSdk.Models.Stocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Build host with Finnhub SDK
var builder = Host.CreateApplicationBuilder(args);

// Configure from environment variable or appsettings.json
var apiKey = Environment.GetEnvironmentVariable("FINNHUB_API_KEY")
    ?? builder.Configuration["Finnhub:ApiKey"]
    ?? throw new InvalidOperationException(
        "API key not found. Set FINNHUB_API_KEY environment variable or configure in appsettings.json");

builder.Services.AddFinnhub(options =>
{
    options.ApiKey = apiKey;
    options.Timeout = TimeSpan.FromSeconds(30);
});

var host = builder.Build();

// Get the Finnhub client
var finnhub = host.Services.GetRequiredService<IFinnhubClient>();

Console.WriteLine("=== FinnhubSdk REST API Samples ===");
Console.WriteLine();

// 1. Stock Quote
Console.WriteLine("1. Getting real-time quote for AAPL...");
try
{
    var quote = await finnhub.Stocks.GetQuoteAsync("AAPL");
    Console.WriteLine($"   Price: ${quote.CurrentPrice}");
    Console.WriteLine($"   Change: {quote.PercentChange:F2}%");
    Console.WriteLine($"   High: ${quote.High}, Low: ${quote.Low}");
    Console.WriteLine($"   Open: ${quote.Open}, Previous Close: ${quote.PreviousClose}");
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 2. Company Profile
Console.WriteLine("2. Getting company profile for MSFT...");
try
{
    var profile = await finnhub.Stocks.GetCompanyProfileAsync("MSFT");
    Console.WriteLine($"   {profile.Name} ({profile.Ticker})");
    Console.WriteLine($"   Industry: {profile.Industry}");
    Console.WriteLine($"   Market Cap: ${profile.MarketCapitalization:N0}");
    Console.WriteLine($"   Exchange: {profile.Exchange}");
    Console.WriteLine($"   Website: {profile.WebUrl}");
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 3. Historical Candles
Console.WriteLine("3. Getting daily candles for GOOGL (last 7 days)...");
try
{
    var request = new CandleRequest
    {
        Symbol = "GOOGL",
        Resolution = CandleResolution.Day,
        From = DateTime.UtcNow.AddDays(-7),
        To = DateTime.UtcNow
    };
    var candles = await finnhub.Stocks.GetCandlesAsync(request);
    foreach (var candle in candles.Take(5))
    {
        Console.WriteLine($"   {candle.Timestamp:yyyy-MM-dd}: O={candle.Open:F2} H={candle.High:F2} L={candle.Low:F2} C={candle.Close:F2} V={candle.Volume:N0}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 4. Symbol Search
Console.WriteLine("4. Searching for 'Tesla'...");
try
{
    var searchResults = await finnhub.Stocks.SearchSymbolsAsync("Tesla");
    foreach (var result in searchResults.Take(3))
    {
        Console.WriteLine($"   {result.Symbol}: {result.Description} ({result.Type})");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 5. Company News
Console.WriteLine("5. Getting recent news for AAPL...");
try
{
    var news = await finnhub.News.GetCompanyNewsAsync("AAPL", DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
    foreach (var article in news.Take(3))
    {
        Console.WriteLine($"   [{article.DateTime:g}] {article.Headline}");
        Console.WriteLine($"      Source: {article.Source}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 6. Market News
Console.WriteLine("6. Getting general market news...");
try
{
    var marketNews = await finnhub.News.GetMarketNewsAsync("general");
    foreach (var article in marketNews.Take(3))
    {
        Console.WriteLine($"   {article.Headline}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 7. News Sentiment
Console.WriteLine("7. Getting news sentiment for AAPL...");
try
{
    var sentiment = await finnhub.News.GetNewsSentimentAsync("AAPL");
    Console.WriteLine($"   Articles this week: {sentiment.Buzz?.ArticlesInLastWeek}");
    Console.WriteLine($"   Buzz score: {sentiment.Buzz?.Buzz:F2}");
    Console.WriteLine($"   Company Score: {sentiment.CompanyNewsScore:F2}");
    Console.WriteLine($"   Sector Average: {sentiment.SectorAverageNewsScore:F2}");
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 8. Basic Financials
Console.WriteLine("8. Getting basic financials for AAPL...");
try
{
    var financials = await finnhub.Stocks.GetBasicFinancialsAsync("AAPL");
    Console.WriteLine($"   Symbol: {financials.Symbol}");
    Console.WriteLine($"   52-Week High: ${financials.Metric?.FiftyTwoWeekHigh:F2}");
    Console.WriteLine($"   52-Week Low: ${financials.Metric?.FiftyTwoWeekLow:F2}");
    Console.WriteLine($"   P/E Ratio (TTM): {financials.Metric?.PeBasicExclExtraTTM:F2}");
    Console.WriteLine($"   Market Cap: ${financials.Metric?.MarketCapitalization:N0}M");
    Console.WriteLine($"   ROE (TTM): {financials.Metric?.RoeTTM:F2}%");
    Console.WriteLine($"   Gross Margin (TTM): {financials.Metric?.GrossMarginTTM:F2}%");
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 9. Company Executives
Console.WriteLine("9. Getting company executives for AAPL...");
try
{
    var executives = await finnhub.Stocks.GetCompanyExecutivesAsync("AAPL");
    foreach (var exec in executives.Take(5))
    {
        Console.WriteLine($"   {exec.Name} - {exec.Title}");
        if (exec.Compensation.HasValue && exec.Compensation > 0)
        {
            Console.WriteLine($"      Compensation: {exec.Compensation:N0} {exec.Currency}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 10. Earnings Calendar
Console.WriteLine("10. Getting earnings calendar (next 7 days)...");
try
{
    var earnings = await finnhub.Stocks.GetEarningsCalendarAsync(DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
    Console.WriteLine($"   Found {earnings.Count} upcoming earnings releases");
    foreach (var entry in earnings.Take(5))
    {
        Console.WriteLine($"   {entry.Symbol}: {entry.Date} ({entry.Hour})");
        if (entry.EpsEstimate.HasValue)
        {
            Console.WriteLine($"      EPS Estimate: {entry.EpsEstimate:F2}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

// 11. Insider Sentiment
Console.WriteLine("11. Getting insider sentiment for AAPL (last 12 months)...");
try
{
    var sentiment = await finnhub.Stocks.GetInsiderSentimentAsync("AAPL", DateTime.UtcNow.AddMonths(-12), DateTime.UtcNow);
    Console.WriteLine($"   Found {sentiment.Count} months of insider sentiment data");
    foreach (var entry in sentiment.Take(5))
    {
        Console.WriteLine($"   {entry.Year}-{entry.Month:D2}: MSPR={entry.Mspr:F2}, Change={entry.Change:N0}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"   Error: {ex.Message}");
}
Console.WriteLine();

Console.WriteLine("=== Samples Complete ===");
