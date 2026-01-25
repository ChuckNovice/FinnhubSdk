using FinnhubSdk.Services.News;

namespace FinnhubSdk.Tests.Integration.Services;

/// <summary>
/// Integration tests for NewsService
/// These tests make real API calls and run sequentially with rate limiting
/// </summary>
[TestClass]
public class NewsServiceIntegrationTests
{
    private static FinnhubTestFixture _fixture = null!;
    private static INewsService _newsService = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        _fixture = new FinnhubTestFixture();
        _newsService = _fixture.GetService<INewsService>();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _fixture?.Dispose();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetCompanyNewsAsync_AAPL_ReturnsNews()
    {
        var from = DateTime.UtcNow.AddDays(-30);
        var to = DateTime.UtcNow;

        var news = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _newsService.GetCompanyNewsAsync("AAPL", from, to));

        Assert.IsNotNull(news);
        Assert.IsTrue(news.Count > 0, "Should return at least one news article for AAPL");

        var firstArticle = news[0];
        Assert.IsFalse(string.IsNullOrEmpty(firstArticle.Headline), "Headline should not be empty");
        Assert.IsFalse(string.IsNullOrEmpty(firstArticle.Source), "Source should not be empty");
        Assert.IsTrue(firstArticle.DateTime > 0, "DateTime should be positive");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetCompanyNewsAsync_MSFT_ReturnsNews()
    {
        var from = DateTime.UtcNow.AddDays(-14);
        var to = DateTime.UtcNow;

        var news = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _newsService.GetCompanyNewsAsync("MSFT", from, to));

        Assert.IsNotNull(news);
        Assert.IsTrue(news.Count > 0, "Should return at least one news article for MSFT");

        var firstArticle = news[0];
        Assert.IsFalse(string.IsNullOrEmpty(firstArticle.Url), "URL should not be empty");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetMarketNewsAsync_General_ReturnsNews()
    {
        var news = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _newsService.GetMarketNewsAsync("general"));

        Assert.IsNotNull(news);
        Assert.IsTrue(news.Count > 0, "Should return at least one general market news article");

        var firstArticle = news[0];
        Assert.IsFalse(string.IsNullOrEmpty(firstArticle.Headline), "Headline should not be empty");
        Assert.IsFalse(string.IsNullOrEmpty(firstArticle.Category), "Category should not be empty");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetMarketNewsAsync_Forex_ReturnsNews()
    {
        var news = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _newsService.GetMarketNewsAsync("forex"));

        Assert.IsNotNull(news);
        // Forex news might be less frequent, so we don't assert count > 0
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetNewsSentimentAsync_AAPL_ReturnsSentiment()
    {
        var sentiment = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _newsService.GetNewsSentimentAsync("AAPL"));

        Assert.IsNotNull(sentiment);
        Assert.AreEqual("AAPL", sentiment.Symbol);
        Assert.IsNotNull(sentiment.Buzz, "Buzz data should not be null");
        Assert.IsTrue(sentiment.Buzz.ArticlesInLastWeek >= 0, "Articles count should be non-negative");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetNewsSentimentAsync_TSLA_ReturnsSentiment()
    {
        var sentiment = await _fixture.ExecuteWithRateLimitAsync(async () =>
            await _newsService.GetNewsSentimentAsync("TSLA"));

        Assert.IsNotNull(sentiment);
        Assert.AreEqual("TSLA", sentiment.Symbol);

        if (sentiment.Sentiment != null)
        {
            var totalPercent = sentiment.Sentiment.BullishPercent + sentiment.Sentiment.BearishPercent;
            Assert.IsTrue(totalPercent <= 1.0m, "Total sentiment percent should not exceed 100%");
        }
    }
}
