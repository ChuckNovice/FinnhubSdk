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

Console.WriteLine("=== Samples Complete ===");
