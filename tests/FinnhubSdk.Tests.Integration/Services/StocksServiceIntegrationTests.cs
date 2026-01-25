using FinnhubSdk.Models.Stocks;
using FinnhubSdk.Services.Stocks;

namespace FinnhubSdk.Tests.Integration.Services;

/// <summary>
/// Integration tests for StocksService
/// These tests make real API calls and run sequentially with rate limiting
/// </summary>
[TestClass]
public class StocksServiceIntegrationTests
{
    private static FinnhubTestFixture _fixture = null!;
    private static IStocksService _stocksService = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        _fixture = new FinnhubTestFixture();
        _stocksService = _fixture.GetService<IStocksService>();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _fixture?.Dispose();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetQuoteAsync_AAPL_ReturnsValidQuote()
    {
        var quote = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _stocksService.GetQuoteAsync("AAPL"));

        Assert.IsNotNull(quote);
        Assert.IsTrue(quote.CurrentPrice > 0, "Current price should be positive");
        Assert.IsTrue(quote.Timestamp > 0, "Timestamp should be positive");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetQuoteAsync_MSFT_ReturnsValidQuote()
    {
        var quote = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _stocksService.GetQuoteAsync("MSFT"));

        Assert.IsNotNull(quote);
        Assert.IsTrue(quote.CurrentPrice > 0, "Current price should be positive");
        Assert.IsTrue(quote.High >= quote.Low, "High should be >= Low");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetCandlesAsync_AAPL_Daily_ReturnsCandles()
    {
        var request = new CandleRequest
        {
            Symbol = "AAPL",
            Resolution = CandleResolution.Day,
            From = DateTime.UtcNow.AddDays(-30),
            To = DateTime.UtcNow
        };

        var candles = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _stocksService.GetCandlesAsync(request));

        Assert.IsNotNull(candles);
        Assert.IsTrue(candles.Count > 0, "Should return at least one candle");

        var firstCandle = candles[0];
        Assert.IsTrue(firstCandle.Close > 0, "Close price should be positive");
        Assert.IsTrue(firstCandle.High >= firstCandle.Low, "High should be >= Low");
        Assert.IsTrue(firstCandle.Volume > 0, "Volume should be positive");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetCompanyProfileAsync_AAPL_ReturnsProfile()
    {
        var profile = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _stocksService.GetCompanyProfileAsync("AAPL"));

        Assert.IsNotNull(profile);
        Assert.AreEqual("AAPL", profile.Ticker);
        Assert.IsFalse(string.IsNullOrEmpty(profile.Name), "Name should not be empty");
        Assert.AreEqual("US", profile.Country);
        Assert.IsTrue(profile.MarketCapitalization > 0, "Market cap should be positive");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetCompanyProfileAsync_MSFT_ReturnsProfile()
    {
        var profile = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _stocksService.GetCompanyProfileAsync("MSFT"));

        Assert.IsNotNull(profile);
        Assert.AreEqual("MSFT", profile.Ticker);
        Assert.IsFalse(string.IsNullOrEmpty(profile.Name), "Name should not be empty");
        Assert.IsFalse(string.IsNullOrEmpty(profile.Exchange), "Exchange should not be empty");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task SearchSymbolsAsync_Apple_ReturnsResults()
    {
        var symbols = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _stocksService.SearchSymbolsAsync("Apple"));

        Assert.IsNotNull(symbols);
        Assert.IsTrue(symbols.Count > 0, "Should find at least one symbol for 'Apple'");

        // Should contain AAPL
        var aapl = symbols.FirstOrDefault(s => s.Symbol == "AAPL");
        Assert.IsNotNull(aapl, "Should find AAPL symbol");
        Assert.IsFalse(string.IsNullOrEmpty(aapl.Description), "Description should not be empty");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task SearchSymbolsAsync_Microsoft_ReturnsResults()
    {
        var symbols = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _stocksService.SearchSymbolsAsync("Microsoft"));

        Assert.IsNotNull(symbols);
        Assert.IsTrue(symbols.Count > 0, "Should find at least one symbol for 'Microsoft'");

        // Should contain MSFT
        var msft = symbols.FirstOrDefault(s => s.Symbol == "MSFT");
        Assert.IsNotNull(msft, "Should find MSFT symbol");
    }
}
