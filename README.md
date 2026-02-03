# FinnhubSdk

A modern .NET client for the [Finnhub Stock API](https://finnhub.io/) with REST and WebSocket support. Access real-time quotes, historical data, company profiles, news, and streaming trades.

[![NuGet](https://img.shields.io/nuget/v/FinnhubSdk.svg)](https://www.nuget.org/packages/FinnhubSdk)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

## Features

- **Stock Market Data** - Real-time quotes, historical candles, company profiles, symbol search, basic financials, executives, peers
- **Corporate Events** - Earnings calendar, insider sentiment, insider transactions, SEC filings
- **News & Sentiment** - Company news, market news, sentiment analysis
- **Forex & Crypto** - Foreign exchange and cryptocurrency candle data
- **Economic Data** - Macroeconomic indicators and time series data
- **WebSocket Streaming** - Real-time trade data with auto-reconnect and async callbacks
- **Modern .NET** - Built for .NET 10.0+ with nullable reference types and async/await
- **Dependency Injection** - First-class support for Microsoft.Extensions.DependencyInjection
- **Multi-Client Support** - Factory pattern for creating clients with different API keys at runtime
- **Resilience** - Built-in retry policies, circuit breaker, and rate limit handling via Polly

## Installation

```bash
dotnet add package FinnhubSdk
```

## Quick Start

### 1. Register the SDK

```csharp
using FinnhubSdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFinnhub(options =>
{
    options.ApiKey = builder.Configuration["Finnhub:ApiKey"]!;
});
```

### 2. Use the Client

```csharp
using FinnhubSdk.Clients;

public class MarketService(IFinnhubClient finnhub)
{
    public async Task<decimal> GetCurrentPrice(string symbol)
    {
        var quote = await finnhub.Stocks.GetQuoteAsync(symbol);
        return quote.CurrentPrice;
    }
}
```

## API Coverage

The SDK provides access to Finnhub API endpoints through organized service interfaces:

### IStocksService

| Method | API Endpoint | Description |
|--------|--------------|-------------|
| `GetQuoteAsync` | `/quote` | Real-time stock quote |
| `GetCandlesAsync` | `/stock/candle` | Historical OHLCV candles |
| `GetCompanyProfileAsync` | `/stock/profile2` | Company profile and details |
| `SearchSymbolsAsync` | `/search` | Search for stock symbols |
| `GetBasicFinancialsAsync` | `/stock/metric` | Key financial metrics (P/E, margins, etc.) |
| `GetCompanyExecutivesAsync` | `/stock/executive` | Company executives and compensation |
| `GetEarningsCalendarAsync` | `/calendar/earnings` | Upcoming earnings releases |
| `GetInsiderSentimentAsync` | `/stock/insider-sentiment` | Monthly insider sentiment data |
| `GetCompanyPeersAsync` | `/stock/peers` | Similar companies |
| `GetInsiderTransactionsAsync` | `/stock/insider-transactions` | Insider buy/sell transactions |
| `GetSecFilingsAsync` | `/stock/filings` | SEC filing documents |

### INewsService

| Method | API Endpoint | Description |
|--------|--------------|-------------|
| `GetCompanyNewsAsync` | `/company-news` | News articles for a company |
| `GetMarketNewsAsync` | `/news` | General market news |
| `GetNewsSentimentAsync` | `/news-sentiment` | News sentiment analysis |

### IForexService

| Method | API Endpoint | Description |
|--------|--------------|-------------|
| `GetCandlesAsync` | `/forex/candle` | Forex OHLCV candles |

### ICryptoService

| Method | API Endpoint | Description |
|--------|--------------|-------------|
| `GetCandlesAsync` | `/crypto/candle` | Cryptocurrency OHLCV candles |

### IEconomicService

| Method | API Endpoint | Description |
|--------|--------------|-------------|
| `GetEconomicDataAsync` | `/economic` | Economic indicator time series |
| `GetEconomicCodesAsync` | `/economic/code` | Available economic indicator codes |

### WebSocket

| Method | Description |
|--------|-------------|
| `SubscribeAsync` | Subscribe to real-time trades |
| `UnsubscribeAsync` | Unsubscribe from trades |
| `ConnectAsync` / `DisconnectAsync` | Manage connection |

## Usage Examples

### Stock Data

```csharp
// Real-time quote
var quote = await finnhub.Stocks.GetQuoteAsync("AAPL");
Console.WriteLine($"Price: ${quote.CurrentPrice}, Change: {quote.PercentChange}%");

// Company profile
var profile = await finnhub.Stocks.GetCompanyProfileAsync("MSFT");
Console.WriteLine($"{profile.Name} - {profile.Industry}");

// Historical candles
var candles = await finnhub.Stocks.GetCandlesAsync(new CandleRequest
{
    Symbol = "GOOGL",
    Resolution = CandleResolution.Day,
    From = DateTime.UtcNow.AddDays(-30),
    To = DateTime.UtcNow
});

// Basic financials
var financials = await finnhub.Stocks.GetBasicFinancialsAsync("AAPL");
Console.WriteLine($"P/E: {financials.Metric?.PeBasicExclExtraTTM}");

// Earnings calendar
var earnings = await finnhub.Stocks.GetEarningsCalendarAsync(
    DateTime.UtcNow,
    DateTime.UtcNow.AddDays(7));

// Company peers
var peers = await finnhub.Stocks.GetCompanyPeersAsync("AAPL");
```

### News

```csharp
// Company news
var news = await finnhub.News.GetCompanyNewsAsync("AAPL",
    DateTime.UtcNow.AddDays(-7),
    DateTime.UtcNow);

// Market news
var marketNews = await finnhub.News.GetMarketNewsAsync("general");
```

### WebSocket Streaming

```csharp
finnhub.WebSocket.OnTradeReceived += (sender, trade) =>
{
    Console.WriteLine($"{trade.Symbol}: ${trade.Price} x {trade.Volume}");
};

await finnhub.WebSocket.ConnectAsync();
await finnhub.WebSocket.SubscribeAsync("AAPL", "MSFT", "GOOGL");
```

## Samples

Ready-to-run sample applications are available in the [`samples/`](samples/) directory:

| Sample | Description |
|--------|-------------|
| [REST API Sample](samples/FinnhubSdk.Samples.Rest) | All 18 REST endpoints: quotes, candles, profiles, news, financials, and more |
| [WebSocket Sample](samples/FinnhubSdk.Samples.WebSocket) | Real-time trade streaming with callbacks |

### Running Samples

```bash
# Set your API key
export FINNHUB_API_KEY=your-api-key    # Linux/macOS
set FINNHUB_API_KEY=your-api-key       # Windows

# Run REST sample
dotnet run --project samples/FinnhubSdk.Samples.Rest

# Run WebSocket sample
dotnet run --project samples/FinnhubSdk.Samples.WebSocket
```

## Configuration

All options can be configured via code or `appsettings.json`:

```csharp
builder.Services.AddFinnhub(options =>
{
    options.ApiKey = "your-api-key";                          // Required
    options.BaseUrl = "https://finnhub.io/api/v1";            // Default
    options.WebSocketUrl = "wss://ws.finnhub.io";             // Default
    options.Timeout = TimeSpan.FromSeconds(30);               // Default
    options.MaxRetries = 3;                                   // Default
    options.RetryDelay = TimeSpan.FromSeconds(1);             // Default
    options.RateLimitStrategy = RateLimitStrategy.ThrowException;
});
```

```json
{
  "Finnhub": {
    "ApiKey": "your-api-key",
    "Timeout": "00:01:00",
    "MaxRetries": 5,
    "RateLimitStrategy": "RetryWithBackoff"
  }
}
```

### Rate Limit Strategies

| Strategy | Description |
|----------|-------------|
| `ThrowException` | Throws `FinnhubRateLimitException` when rate limited |
| `RetryWithBackoff` | Automatically retries with exponential backoff |
| `QueueRequest` | Queues requests to stay within rate limits |

## Multi-Client Support

For scenarios where you need multiple clients with different API keys (e.g., multi-tenant applications), use the factory pattern:

### Stateless Factory

Use `IFinnhubClientFactory` when you want full control over client lifecycle:

```csharp
// Registration
builder.Services.AddFinnhubFactory(options =>
{
    // Configure shared defaults (optional)
    options.Timeout = TimeSpan.FromSeconds(60);
});
```

```csharp
// Usage - each call creates a new client
public class StockService(IFinnhubClientFactory factory)
{
    public async Task<Quote> GetQuoteAsync(string userApiKey, string symbol)
    {
        var client = factory.CreateClient(userApiKey);
        return await client.Stocks.GetQuoteAsync(symbol);
    }
}
```

### Cached Factory (Recommended for Multi-Tenant)

Use `ICachedFinnhubClientFactory` for automatic caching, thread safety, and disposal:

```csharp
// Registration
builder.Services.AddCachedFinnhubFactory(
    configureCache: options =>
    {
        options.CacheOptions.SizeLimit = 500;                    // Max cached clients
        options.EntryOptions.SlidingExpiration = TimeSpan.FromMinutes(60);  // Evict after 1 hour of inactivity
    },
    configureClient: options =>
    {
        options.Timeout = TimeSpan.FromSeconds(60);
    });
```

```csharp
// Usage - same API key returns cached instance
public class StockService(ICachedFinnhubClientFactory factory, IAccountRepository accounts)
{
    public async Task<Quote> GetQuoteAsync(int userId, string symbol, CancellationToken ct)
    {
        var account = await accounts.GetAsync(userId, ct);
        var client = factory.GetOrCreateClient(account.ApiKey);  // Thread-safe, cached
        return await client.Stocks.GetQuoteAsync(symbol, ct);
    }

    public async Task OnUserLogoutAsync(int userId, CancellationToken ct)
    {
        var account = await accounts.GetAsync(userId, ct);
        await factory.RemoveAsync(account.ApiKey);  // Removes from cache and disposes WebSocket
    }
}
```

### Factory Features

| Feature | `IFinnhubClientFactory` | `ICachedFinnhubClientFactory` |
|---------|-------------------------|-------------------------------|
| Creates new instances | Always | On first call per key |
| Thread-safe | N/A | Yes |
| Automatic caching | No | Yes |
| Automatic disposal | No | Yes (on eviction) |
| Custom caching | Implement your own | Built-in with MemoryCache |

## Error Handling

The SDK provides specific exception types:

```csharp
using FinnhubSdk.Exceptions;

try
{
    var quote = await finnhub.Stocks.GetQuoteAsync("AAPL");
}
catch (FinnhubAuthenticationException)
{
    // Invalid or missing API key (401)
}
catch (FinnhubRateLimitException ex)
{
    // Rate limit exceeded (429) - check ex.RetryAfter
}
catch (FinnhubApiException ex)
{
    // Other API errors (400, 404, 500) - check ex.StatusCode
}
catch (FinnhubWebSocketException)
{
    // WebSocket connection errors
}
```

### Exception Hierarchy

- `FinnhubException` (base)
  - `FinnhubApiException` - HTTP errors
  - `FinnhubRateLimitException` - Rate limit exceeded (429)
  - `FinnhubAuthenticationException` - Auth failed (401)
  - `FinnhubWebSocketException` - WebSocket errors

## WebSocket Features

- **Auto-reconnect** with exponential backoff (2s, 4s, 8s, 16s, 32s)
- **Auto-resubscribe** to symbols after reconnection
- **Async callbacks** for trade data, state changes, and errors
- **Connection states**: Disconnected, Connecting, Connected, Reconnecting, Failed

## Resilience

Built-in Polly policies:
- **Retry**: 3 attempts with exponential backoff for transient errors
- **Circuit Breaker**: Opens after 5 consecutive failures, breaks for 30 seconds
- **Timeout**: Configurable per-request timeout

## Requirements

- .NET 10.0 or later
- Finnhub API key ([Get one free](https://finnhub.io/register))

## License

Apache License 2.0 - see [LICENSE](LICENSE) for details.

## Links

- [Finnhub API Documentation](https://finnhub.io/docs/api)
- [NuGet Package](https://www.nuget.org/packages/FinnhubSdk)
- [GitHub Repository](https://github.com/ChuckNovice/FinnhubSdk)
