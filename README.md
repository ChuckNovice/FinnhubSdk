# FinnhubSdk

A modern .NET client for the [Finnhub Stock API](https://finnhub.io/) with REST and WebSocket support. Access real-time quotes, historical data, company profiles, news, and streaming trades.

[![NuGet](https://img.shields.io/nuget/v/FinnhubSdk.svg)](https://www.nuget.org/packages/FinnhubSdk)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

## Features

- **Stock Market Data** - Real-time quotes, historical candles (OHLCV), company profiles, symbol search
- **News** - Company news, market news, sentiment analysis
- **WebSocket Streaming** - Real-time trade data with auto-reconnect and async callbacks
- **Modern .NET** - Built for .NET 9.0+ with nullable reference types and async/await
- **Dependency Injection** - First-class support for Microsoft.Extensions.DependencyInjection
- **Resilience** - Built-in retry policies, circuit breaker, and rate limit handling via Polly

## Installation

```bash
dotnet add package FinnhubSdk
```

## Quick Start

### 1. Register the SDK with Dependency Injection

```csharp
using FinnhubSdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Option 1: Configure inline
builder.Services.AddFinnhub(options =>
{
    options.ApiKey = builder.Configuration["Finnhub:ApiKey"]!;
});

// Option 2: Bind from configuration section
builder.Services.AddFinnhub(builder.Configuration.GetSection("Finnhub"));
```

### 2. Add Configuration (appsettings.json)

```json
{
  "Finnhub": {
    "ApiKey": "your-api-key-here"
  }
}
```

### 3. Use the Client

```csharp
using FinnhubSdk.Clients;

public class MarketService
{
    private readonly IFinnhubClient _finnhub;

    public MarketService(IFinnhubClient finnhub)
    {
        _finnhub = finnhub;
    }

    public async Task<decimal> GetCurrentPrice(string symbol)
    {
        var quote = await _finnhub.Stocks.GetQuoteAsync(symbol);
        return quote.CurrentPrice;
    }
}
```

## REST API Examples

### Stock Quotes

```csharp
// Get real-time quote for a symbol
var quote = await finnhub.Stocks.GetQuoteAsync("AAPL");

Console.WriteLine($"Price: ${quote.CurrentPrice}");
Console.WriteLine($"Change: {quote.PercentChange:F2}%");
Console.WriteLine($"High: ${quote.High}, Low: ${quote.Low}");
```

### Historical Candles

```csharp
using FinnhubSdk.Models.Stocks;

// Get daily candles for the past week
var request = new CandleRequest
{
    Symbol = "MSFT",
    Resolution = CandleResolution.Day,
    From = DateTime.UtcNow.AddDays(-7),
    To = DateTime.UtcNow
};

var candles = await finnhub.Stocks.GetCandlesAsync(request);

foreach (var candle in candles)
{
    Console.WriteLine($"{candle.Timestamp:d}: O={candle.Open} H={candle.High} L={candle.Low} C={candle.Close}");
}
```

### Company Profile

```csharp
var profile = await finnhub.Stocks.GetCompanyProfileAsync("GOOGL");

Console.WriteLine($"{profile.Name} ({profile.Ticker})");
Console.WriteLine($"Industry: {profile.Industry}");
Console.WriteLine($"Market Cap: ${profile.MarketCapitalization:N0}");
Console.WriteLine($"Exchange: {profile.Exchange}");
```

### Symbol Search

```csharp
var results = await finnhub.Stocks.SearchSymbolsAsync("Tesla");

foreach (var symbol in results)
{
    Console.WriteLine($"{symbol.Symbol}: {symbol.Description}");
}
```

### Company News

```csharp
var news = await finnhub.News.GetCompanyNewsAsync(
    "AAPL",
    DateTime.UtcNow.AddDays(-7),
    DateTime.UtcNow);

foreach (var article in news.Take(5))
{
    Console.WriteLine($"{article.DateTime:g}: {article.Headline}");
    Console.WriteLine($"  Source: {article.Source}");
}
```

### Market News

```csharp
// Categories: general, forex, crypto, merger
var news = await finnhub.News.GetMarketNewsAsync("general");

foreach (var article in news.Take(5))
{
    Console.WriteLine($"{article.Headline}");
}
```

### News Sentiment

```csharp
var sentiment = await finnhub.News.GetNewsSentimentAsync("AAPL");

Console.WriteLine($"Articles this week: {sentiment.ArticlesInLastWeek}");
Console.WriteLine($"Buzz: {sentiment.Buzz?.Buzz:F2}");
Console.WriteLine($"Company Score: {sentiment.CompanyNewsScore:F2}");
```

## WebSocket Streaming

The WebSocket client provides real-time trade streaming with automatic reconnection and async callbacks.

### Basic Usage

```csharp
using FinnhubSdk.WebSocket;

public class TradeMonitor
{
    private readonly IFinnhubWebSocketClient _wsClient;

    public TradeMonitor(IFinnhubWebSocketClient wsClient)
    {
        _wsClient = wsClient;

        // Set up async callbacks (not events - allows await)
        _wsClient.OnTradeReceived = HandleTradeAsync;
        _wsClient.OnConnectionStateChanged = HandleStateChangeAsync;
        _wsClient.OnErrorOccurred = HandleErrorAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Connect to WebSocket server
        await _wsClient.ConnectAsync(cancellationToken);

        // Subscribe to symbols
        await _wsClient.SubscribeAsync("AAPL", cancellationToken);
        await _wsClient.SubscribeAsync("MSFT", cancellationToken);

        // Or subscribe to multiple at once
        await _wsClient.SubscribeAsync(["GOOGL", "AMZN", "META"], cancellationToken);
    }

    private async Task HandleTradeAsync(TradeMessage message)
    {
        foreach (var trade in message.Data)
        {
            Console.WriteLine($"Trade: {trade.Symbol} @ ${trade.Price} (Vol: {trade.Volume})");

            // You can await async operations here (e.g., save to database)
            await SaveTradeAsync(trade);
        }
    }

    private Task HandleStateChangeAsync(ConnectionState state)
    {
        Console.WriteLine($"Connection state: {state}");
        return Task.CompletedTask;
    }

    private Task HandleErrorAsync(Exception ex)
    {
        Console.WriteLine($"WebSocket error: {ex.Message}");
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _wsClient.UnsubscribeAllAsync(cancellationToken);
        await _wsClient.DisconnectAsync(cancellationToken);
    }
}
```

### ASP.NET Core Background Service

```csharp
public class RealtimeTradeService : BackgroundService
{
    private readonly IFinnhubWebSocketClient _wsClient;
    private readonly ILogger<RealtimeTradeService> _logger;

    public RealtimeTradeService(
        IFinnhubWebSocketClient wsClient,
        ILogger<RealtimeTradeService> logger)
    {
        _wsClient = wsClient;
        _logger = logger;

        _wsClient.OnTradeReceived = async message =>
        {
            foreach (var trade in message.Data)
            {
                _logger.LogInformation(
                    "Trade: {Symbol} @ {Price}",
                    trade.Symbol,
                    trade.Price);
            }
            await Task.CompletedTask;
        };

        _wsClient.OnConnectionStateChanged = state =>
        {
            _logger.LogInformation("WebSocket state: {State}", state);
            return Task.CompletedTask;
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _wsClient.ConnectAsync(stoppingToken);
        await _wsClient.SubscribeAsync(["AAPL", "MSFT", "GOOGL"], stoppingToken);

        // Keep running until cancelled
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _wsClient.DisconnectAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
```

### Connection States

The WebSocket client tracks these connection states:

| State | Description |
|-------|-------------|
| `Disconnected` | Not connected to server |
| `Connecting` | Connection attempt in progress |
| `Connected` | Connected and ready |
| `Reconnecting` | Auto-reconnecting after disconnect |
| `Failed` | Connection failed after all retry attempts |

### Auto-Reconnection

The WebSocket client automatically:
- Detects disconnection (network errors, server close)
- Reconnects with exponential backoff (2s, 4s, 8s, 16s, 32s max)
- Resubscribes to all previously subscribed symbols
- Retries up to 5 times before entering `Failed` state

## Configuration Options

All options can be configured via code or `appsettings.json`:

```csharp
builder.Services.AddFinnhub(options =>
{
    // Required
    options.ApiKey = "your-api-key";

    // Optional - defaults shown
    options.BaseUrl = "https://finnhub.io/api/v1";
    options.WebSocketUrl = "wss://ws.finnhub.io";
    options.AuthMethod = AuthenticationMethod.Header; // or QueryString
    options.TimeoutSeconds = 30;
    options.MaxRetries = 3;
    options.RetryDelayMilliseconds = 1000;
    options.EnableLogging = true;
    options.RateLimitStrategy = RateLimitStrategy.ThrowException;
});
```

Or via configuration:

```json
{
  "Finnhub": {
    "ApiKey": "your-api-key",
    "TimeoutSeconds": 60,
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

## Error Handling

The SDK provides specific exception types for different error scenarios:

```csharp
using FinnhubSdk.Exceptions;

try
{
    var quote = await finnhub.Stocks.GetQuoteAsync("INVALID");
}
catch (FinnhubAuthenticationException ex)
{
    // Invalid or missing API key
    Console.WriteLine("Authentication failed. Check your API key.");
}
catch (FinnhubRateLimitException ex)
{
    // Rate limit exceeded (429)
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}");
}
catch (FinnhubApiException ex)
{
    // Other API errors (400, 404, 500, etc.)
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
}
catch (FinnhubWebSocketException ex)
{
    // WebSocket-specific errors
    Console.WriteLine($"WebSocket error: {ex.Message}");
}
catch (FinnhubException ex)
{
    // Base class for all SDK exceptions
    Console.WriteLine($"Finnhub error: {ex.Message}");
}
```

### Exception Hierarchy

- `FinnhubException` (abstract base)
  - `FinnhubApiException` - HTTP errors (400, 404, 500, etc.)
  - `FinnhubRateLimitException` - Rate limit exceeded (429)
  - `FinnhubAuthenticationException` - Authentication failed (401)
  - `FinnhubWebSocketException` - WebSocket connection errors

## Rate Limiting

Finnhub's free tier allows 60 API calls per minute. The SDK handles rate limits in several ways:

1. **Retry-After Header**: When rate limited, the SDK respects the `Retry-After` header from Finnhub
2. **Exponential Backoff**: If no `Retry-After` header, uses exponential backoff
3. **Configurable Strategy**: Choose between throwing exceptions, auto-retry, or request queuing

### Best Practices

- Use the WebSocket for real-time data instead of polling REST endpoints
- Cache responses when appropriate (company profiles change infrequently)
- Consider upgrading to a paid Finnhub plan for higher rate limits
- Batch symbol subscriptions rather than individual calls

## Resilience Policies

The SDK includes built-in Polly policies:

- **Retry**: 3 attempts with exponential backoff for transient errors (5xx, timeouts)
- **Circuit Breaker**: Opens after 5 consecutive failures, breaks for 30 seconds
- **Timeout**: 30 seconds per request (configurable)

These policies can be customized via the options.

## Requirements

- .NET 9.0 or later
- Finnhub API key ([Get one free](https://finnhub.io/register))

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Links

- [Finnhub API Documentation](https://finnhub.io/docs/api)
- [NuGet Package](https://www.nuget.org/packages/FinnhubSdk)
- [GitHub Repository](https://github.com/ChuckNovice/FinnhubSdk)
