using FinnhubSdk.Services.Forex;
using FinnhubSdk.Services.News;
using FinnhubSdk.Services.Stocks;
using FinnhubSdk.WebSocket;

namespace FinnhubSdk.Clients;

/// <summary>
/// Main client facade for accessing all Finnhub API services
/// </summary>
internal sealed class FinnhubClient : IFinnhubClient
{
    /// <inheritdoc/>
    public IStocksService Stocks { get; }

    /// <inheritdoc/>
    public INewsService News { get; }

    /// <inheritdoc/>
    public IForexService Forex { get; }

    /// <inheritdoc/>
    public IFinnhubWebSocketClient WebSocket { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FinnhubClient"/> class
    /// </summary>
    /// <param name="stocks">Stocks service</param>
    /// <param name="news">News service</param>
    /// <param name="forex">Forex service</param>
    /// <param name="webSocket">WebSocket client</param>
    public FinnhubClient(
        IStocksService stocks,
        INewsService news,
        IForexService forex,
        IFinnhubWebSocketClient webSocket)
    {
        Stocks = stocks ?? throw new ArgumentNullException(nameof(stocks));
        News = news ?? throw new ArgumentNullException(nameof(news));
        Forex = forex ?? throw new ArgumentNullException(nameof(forex));
        WebSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
    }
}
