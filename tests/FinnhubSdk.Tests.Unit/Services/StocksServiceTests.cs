using FinnhubSdk.Clients;
using FinnhubSdk.Models.Stocks;
using FinnhubSdk.Services.Stocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinnhubSdk.Tests.Unit.Services;

[TestClass]
public class StocksServiceTests
{
    private Mock<FinnhubHttpClient> _mockHttpClient = null!;
    private Mock<ILogger<StocksService>> _mockLogger = null!;
    private StocksService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockHttpClient = new Mock<FinnhubHttpClient>(
            MockBehavior.Strict,
            Mock.Of<HttpClient>(),
            Mock.Of<Microsoft.Extensions.Options.IOptions<FinnhubSdk.Configuration.FinnhubOptions>>(),
            Mock.Of<ILogger<FinnhubHttpClient>>());

        _mockLogger = new Mock<ILogger<StocksService>>();
        _service = new StocksService(_mockHttpClient.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetQuoteAsync_ValidSymbol_ReturnsQuote()
    {
        // Arrange
        var symbol = "AAPL";
        var expectedQuote = new Quote
        {
            CurrentPrice = 150.25m,
            High = 152.00m,
            Low = 149.00m,
            Open = 150.00m,
            PreviousClose = 149.50m,
            Change = 0.75m,
            PercentChange = 0.50m,
            Timestamp = 1234567890
        };

        _mockHttpClient
            .Setup(x => x.GetAsync<Quote>(
                It.Is<string>(s => s.Contains("quote") && s.Contains(symbol)),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedQuote);

        // Act
        var result = await _service.GetQuoteAsync(symbol);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedQuote.CurrentPrice, result.CurrentPrice);
        Assert.AreEqual(expectedQuote.High, result.High);
        Assert.AreEqual(expectedQuote.Low, result.Low);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task GetQuoteAsync_NullSymbol_ThrowsArgumentException()
    {
        // Act
        await _service.GetQuoteAsync(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task GetQuoteAsync_EmptySymbol_ThrowsArgumentException()
    {
        // Act
        await _service.GetQuoteAsync(string.Empty);
    }

    [TestMethod]
    public async Task GetCandlesAsync_ValidRequest_ReturnsCandles()
    {
        // Arrange
        var request = new CandleRequest
        {
            Symbol = "AAPL",
            Resolution = CandleResolution.Day,
            From = DateTime.UtcNow.AddDays(-7),
            To = DateTime.UtcNow
        };

        var candleResponse = new CandleResponse
        {
            Status = "ok",
            Close = new[] { 150.0m, 151.0m, 152.0m },
            High = new[] { 151.0m, 152.0m, 153.0m },
            Low = new[] { 149.0m, 150.0m, 151.0m },
            Open = new[] { 149.5m, 150.5m, 151.5m },
            Volume = new[] { 1000000L, 1100000L, 1200000L },
            Timestamp = new[] { 1609459200L, 1609545600L, 1609632000L }
        };

        _mockHttpClient
            .Setup(x => x.GetAsync<CandleResponse>(
                It.Is<string>(s => s.Contains("stock/candle") && s.Contains(request.Symbol)),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(candleResponse);

        // Act
        var result = await _service.GetCandlesAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(150.0m, result[0].Close);
        Assert.AreEqual(151.0m, result[1].Close);
        Assert.AreEqual(152.0m, result[2].Close);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task GetCandlesAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act
        await _service.GetCandlesAsync(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task GetCandlesAsync_FromAfterTo_ThrowsArgumentException()
    {
        // Arrange
        var request = new CandleRequest
        {
            Symbol = "AAPL",
            Resolution = CandleResolution.Day,
            From = DateTime.UtcNow,
            To = DateTime.UtcNow.AddDays(-7)
        };

        // Act
        await _service.GetCandlesAsync(request);
    }

    [TestMethod]
    public async Task GetCompanyProfileAsync_ValidSymbol_ReturnsProfile()
    {
        // Arrange
        var symbol = "AAPL";
        var expectedProfile = new CompanyProfile
        {
            Name = "Apple Inc",
            Ticker = "AAPL",
            Country = "US",
            Currency = "USD",
            Exchange = "NASDAQ",
            Industry = "Technology",
            MarketCapitalization = 2500000000000m
        };

        _mockHttpClient
            .Setup(x => x.GetAsync<CompanyProfile>(
                It.Is<string>(s => s.Contains("stock/profile2") && s.Contains(symbol)),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _service.GetCompanyProfileAsync(symbol);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedProfile.Name, result.Name);
        Assert.AreEqual(expectedProfile.Ticker, result.Ticker);
        Assert.AreEqual(expectedProfile.Country, result.Country);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task GetCompanyProfileAsync_NullSymbol_ThrowsArgumentException()
    {
        // Act
        await _service.GetCompanyProfileAsync(null!);
    }

    [TestMethod]
    public async Task SearchSymbolsAsync_ValidQuery_ReturnsSymbols()
    {
        // Arrange
        var query = "Apple";
        var searchResponse = new SymbolSearchResponse
        {
            Count = 2,
            Result = new[]
            {
                new StockSymbol
                {
                    Symbol = "AAPL",
                    DisplaySymbol = "AAPL",
                    Description = "Apple Inc",
                    Type = "Common Stock"
                },
                new StockSymbol
                {
                    Symbol = "AAPL.SW",
                    DisplaySymbol = "AAPL.SW",
                    Description = "Apple Inc - Swiss Exchange",
                    Type = "Common Stock"
                }
            }
        };

        _mockHttpClient
            .Setup(x => x.GetAsync<SymbolSearchResponse>(
                It.Is<string>(s => s.Contains("search") && s.Contains(query)),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResponse);

        // Act
        var result = await _service.SearchSymbolsAsync(query);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("AAPL", result[0].Symbol);
        Assert.AreEqual("AAPL.SW", result[1].Symbol);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task SearchSymbolsAsync_NullQuery_ThrowsArgumentException()
    {
        // Act
        await _service.SearchSymbolsAsync(null!);
    }

    [TestMethod]
    public async Task SearchSymbolsAsync_NoResults_ReturnsEmptyList()
    {
        // Arrange
        var query = "NonExistentCompany";
        var searchResponse = new SymbolSearchResponse
        {
            Count = 0,
            Result = Array.Empty<StockSymbol>()
        };

        _mockHttpClient
            .Setup(x => x.GetAsync<SymbolSearchResponse>(
                It.Is<string>(s => s.Contains("search") && s.Contains(query)),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResponse);

        // Act
        var result = await _service.SearchSymbolsAsync(query);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }
}
