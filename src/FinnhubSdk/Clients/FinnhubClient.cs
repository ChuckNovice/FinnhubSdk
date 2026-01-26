namespace FinnhubSdk.Clients;

using FinnhubSdk.Services.Crypto;
using FinnhubSdk.Services.Economic;
using FinnhubSdk.Services.Forex;
using FinnhubSdk.Services.News;
using FinnhubSdk.Services.Stocks;
using FinnhubSdk.WebSocket;

/// <summary>
/// Main client facade for accessing all Finnhub API services
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FinnhubClient"/> class
/// </remarks>
/// <param name="stocks">Stocks service</param>
/// <param name="news">News service</param>
/// <param name="forex">Forex service</param>
/// <param name="crypto">Crypto service</param>
/// <param name="economic">Economic service</param>
/// <param name="webSocket">WebSocket client</param>
internal sealed class FinnhubClient(
    IStocksService stocks,
    INewsService news,
    IForexService forex,
    ICryptoService crypto,
    IEconomicService economic,
    IFinnhubWebSocketClient webSocket) : IFinnhubClient
{
    /// <inheritdoc/>
    public IStocksService Stocks { get; } = stocks ?? throw new ArgumentNullException(nameof(stocks));

    /// <inheritdoc/>
    public INewsService News { get; } = news ?? throw new ArgumentNullException(nameof(news));

    /// <inheritdoc/>
    public IForexService Forex { get; } = forex ?? throw new ArgumentNullException(nameof(forex));

    /// <inheritdoc/>
    public ICryptoService Crypto { get; } = crypto ?? throw new ArgumentNullException(nameof(crypto));

    /// <inheritdoc/>
    public IEconomicService Economic { get; } = economic ?? throw new ArgumentNullException(nameof(economic));

    /// <inheritdoc/>
    public IFinnhubWebSocketClient WebSocket { get; } = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
}
