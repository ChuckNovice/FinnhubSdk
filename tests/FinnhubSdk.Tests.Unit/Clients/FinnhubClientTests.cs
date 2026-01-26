using FinnhubSdk.Clients;
using FinnhubSdk.Services.Crypto;
using FinnhubSdk.Services.Economic;
using FinnhubSdk.Services.Forex;
using FinnhubSdk.Services.News;
using FinnhubSdk.Services.Stocks;
using FinnhubSdk.WebSocket;
using Moq;

namespace FinnhubSdk.Tests.Unit.Clients;

[TestClass]
public class FinnhubClientTests
{
    private Mock<IStocksService> _mockStocksService = null!;
    private Mock<INewsService> _mockNewsService = null!;
    private Mock<IForexService> _mockForexService = null!;
    private Mock<ICryptoService> _mockCryptoService = null!;
    private Mock<IEconomicService> _mockEconomicService = null!;
    private Mock<IFinnhubWebSocketClient> _mockWebSocketClient = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockStocksService = new Mock<IStocksService>();
        _mockNewsService = new Mock<INewsService>();
        _mockForexService = new Mock<IForexService>();
        _mockCryptoService = new Mock<ICryptoService>();
        _mockEconomicService = new Mock<IEconomicService>();
        _mockWebSocketClient = new Mock<IFinnhubWebSocketClient>();
    }

    [TestMethod]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        // Act
        var client = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);

        // Assert
        Assert.IsNotNull(client);
        Assert.AreSame(_mockStocksService.Object, client.Stocks);
        Assert.AreSame(_mockNewsService.Object, client.News);
        Assert.AreSame(_mockForexService.Object, client.Forex);
        Assert.AreSame(_mockCryptoService.Object, client.Crypto);
        Assert.AreSame(_mockEconomicService.Object, client.Economic);
        Assert.AreSame(_mockWebSocketClient.Object, client.WebSocket);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullStocksService_ThrowsArgumentNullException()
    {
        // Act
        _ = new FinnhubClient(
            null!,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullNewsService_ThrowsArgumentNullException()
    {
        // Act
        _ = new FinnhubClient(
            _mockStocksService.Object,
            null!,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullForexService_ThrowsArgumentNullException()
    {
        // Act
        _ = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            null!,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullCryptoService_ThrowsArgumentNullException()
    {
        // Act
        _ = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            null!,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullEconomicService_ThrowsArgumentNullException()
    {
        // Act
        _ = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            null!,
            _mockWebSocketClient.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullWebSocketClient_ThrowsArgumentNullException()
    {
        // Act
        _ = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            null!);
    }

    [TestMethod]
    public void Stocks_ReturnsInjectedService()
    {
        // Arrange
        var client = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);

        // Act & Assert
        Assert.AreSame(_mockStocksService.Object, client.Stocks);
    }

    [TestMethod]
    public void News_ReturnsInjectedService()
    {
        // Arrange
        var client = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);

        // Act & Assert
        Assert.AreSame(_mockNewsService.Object, client.News);
    }

    [TestMethod]
    public void Forex_ReturnsInjectedService()
    {
        // Arrange
        var client = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);

        // Act & Assert
        Assert.AreSame(_mockForexService.Object, client.Forex);
    }

    [TestMethod]
    public void Crypto_ReturnsInjectedService()
    {
        // Arrange
        var client = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);

        // Act & Assert
        Assert.AreSame(_mockCryptoService.Object, client.Crypto);
    }

    [TestMethod]
    public void Economic_ReturnsInjectedService()
    {
        // Arrange
        var client = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);

        // Act & Assert
        Assert.AreSame(_mockEconomicService.Object, client.Economic);
    }

    [TestMethod]
    public void WebSocket_ReturnsInjectedClient()
    {
        // Arrange
        var client = new FinnhubClient(
            _mockStocksService.Object,
            _mockNewsService.Object,
            _mockForexService.Object,
            _mockCryptoService.Object,
            _mockEconomicService.Object,
            _mockWebSocketClient.Object);

        // Act & Assert
        Assert.AreSame(_mockWebSocketClient.Object, client.WebSocket);
    }
}
