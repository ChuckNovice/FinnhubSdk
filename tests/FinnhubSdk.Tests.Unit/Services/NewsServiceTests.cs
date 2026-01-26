namespace FinnhubSdk.Tests.Unit.Services;

using FinnhubSdk.Clients;
using FinnhubSdk.Models.News;
using FinnhubSdk.Services.News;
using Microsoft.Extensions.Logging;
using Moq;

[TestClass]
public class NewsServiceTests
{
    private Mock<FinnhubHttpClient> _mockHttpClient = null!;
    private Mock<ILogger<NewsService>> _mockLogger = null!;
    private NewsService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockHttpClient = new Mock<FinnhubHttpClient>(
            MockBehavior.Strict,
            Mock.Of<HttpClient>(),
            Mock.Of<Microsoft.Extensions.Options.IOptions<Configuration.FinnhubOptions>>(),
            Mock.Of<ILogger<FinnhubHttpClient>>());

        _mockLogger = new Mock<ILogger<NewsService>>();
        _service = new NewsService(_mockHttpClient.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetCompanyNewsAsync_ValidParameters_ReturnsNews()
    {
        // Arrange
        var symbol = "AAPL";
        var from = DateTime.UtcNow.AddDays(-7);
        var to = DateTime.UtcNow;
        var expectedNews = new[]
        {
            new NewsArticle
            {
                Id = 1,
                Category = "company",
                DateTime = 1234567890,
                Headline = "Apple announces new product",
                Source = "Reuters",
                Summary = "Apple Inc. announced...",
                Url = "https://example.com/news1"
            },
            new NewsArticle
            {
                Id = 2,
                Category = "company",
                DateTime = 1234567900,
                Headline = "Apple stock rises",
                Source = "Bloomberg",
                Summary = "Apple stock climbed...",
                Url = "https://example.com/news2"
            }
        };

        _mockHttpClient
            .Setup(x => x.GetAsync<NewsArticle[]>(
                It.Is<string>(s => s.Contains("company-news") && s.Contains(symbol)),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedNews);

        // Act
        var result = await _service.GetCompanyNewsAsync(symbol, from, to);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Apple announces new product", result[0].Headline);
        Assert.AreEqual("Apple stock rises", result[1].Headline);
    }

    [TestMethod]
    public async Task GetCompanyNewsAsync_NullSymbol_ThrowsArgumentException()
    {
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                // Act
                await _service.GetCompanyNewsAsync(null!, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow));
    }

    [TestMethod]
    public async Task GetCompanyNewsAsync_FromAfterTo_ThrowsArgumentException()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = DateTime.UtcNow.AddDays(-7);
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>

                // Act
                await _service.GetCompanyNewsAsync("AAPL", from, to));
    }

    [TestMethod]
    public async Task GetMarketNewsAsync_ValidCategory_ReturnsNews()
    {
        // Arrange
        var category = "general";
        var expectedNews = new[]
        {
            new NewsArticle
            {
                Id = 1,
                Category = "general",
                DateTime = 1234567890,
                Headline = "Market hits all-time high",
                Source = "CNBC",
                Summary = "Stock market reached...",
                Url = "https://example.com/news1"
            }
        };

        _mockHttpClient
            .Setup(x => x.GetAsync<NewsArticle[]>(
                It.Is<string>(s => s.Contains("news") && s.Contains(category)),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedNews);

        // Act
        var result = await _service.GetMarketNewsAsync(category);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Market hits all-time high", result[0].Headline);
    }

    [TestMethod]
    public async Task GetMarketNewsAsync_NullCategory_ThrowsArgumentException()
    {
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                // Act
                await _service.GetMarketNewsAsync(null!));
    }

    [TestMethod]
    public async Task GetNewsSentimentAsync_ValidSymbol_ReturnsSentiment()
    {
        // Arrange
        var symbol = "AAPL";
        var expectedSentiment = new NewsSentiment
        {
            Symbol = symbol,
            Buzz = new SentimentBuzz
            {
                ArticlesInLastWeek = 25,
                Buzz = 0.85m,
                WeeklyAverage = 20.5m
            },
            CompanyNewsScore = 0.75m,
            SectorAverageBullishPercent = 0.65m,
            SectorAverageNewsScore = 0.70m,
            Sentiment = new SentimentData
            {
                BearishPercent = 0.30m,
                BullishPercent = 0.70m
            }
        };

        _mockHttpClient
            .Setup(x => x.GetAsync<NewsSentiment>(
                It.Is<string>(s => s.Contains("news-sentiment") && s.Contains(symbol)),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSentiment);

        // Act
        var result = await _service.GetNewsSentimentAsync(symbol);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(symbol, result.Symbol);
        Assert.IsNotNull(result.Buzz);
        Assert.AreEqual(25, result.Buzz.ArticlesInLastWeek);
        Assert.IsNotNull(result.Sentiment);
        Assert.AreEqual(0.70m, result.Sentiment.BullishPercent);
    }

    [TestMethod]
    public async Task GetNewsSentimentAsync_NullSymbol_ThrowsArgumentException()
    {
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                // Act
                await _service.GetNewsSentimentAsync(null!));
    }
}
