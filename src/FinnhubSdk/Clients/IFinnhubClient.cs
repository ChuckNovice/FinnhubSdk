using FinnhubSdk.Services.Crypto;
using FinnhubSdk.Services.Forex;
using FinnhubSdk.Services.News;
using FinnhubSdk.Services.Stocks;
using FinnhubSdk.WebSocket;

namespace FinnhubSdk.Clients;

/// <summary>
/// Main client facade for accessing all Finnhub API services
/// </summary>
public interface IFinnhubClient
{
    /// <summary>
    /// Gets the stocks service for accessing stock market data
    /// </summary>
    IStocksService Stocks { get; }

    /// <summary>
    /// Gets the news service for accessing market and company news
    /// </summary>
    INewsService News { get; }

    /// <summary>
    /// Gets the forex service for accessing foreign exchange market data
    /// </summary>
    IForexService Forex { get; }

    /// <summary>
    /// Gets the crypto service for accessing cryptocurrency market data
    /// </summary>
    ICryptoService Crypto { get; }

    /// <summary>
    /// Gets the WebSocket client for real-time trade streaming
    /// </summary>
    IFinnhubWebSocketClient WebSocket { get; }
}
