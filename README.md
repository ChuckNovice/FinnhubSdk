# FinnhubSdk

A modern .NET client for the [Finnhub Stock API](https://finnhub.io/) with REST and WebSocket support. Access real-time quotes, historical data, company profiles, news, and streaming trades.

[![NuGet](https://img.shields.io/nuget/v/FinnhubSdk.svg)](https://www.nuget.org/packages/FinnhubSdk)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

## Features

- **Stock Market Data** - Real-time quotes, historical candles (OHLCV), company profiles, symbol search
- **News** - Company news, market news, sentiment analysis
- **WebSocket Streaming** - Real-time trade data with auto-reconnect and async callbacks
- **Modern .NET** - Built for .NET 10.0+ with nullable reference types and async/await
- **Dependency Injection** - First-class support for Microsoft.Extensions.DependencyInjection
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

## Samples

Ready-to-run sample applications are available in the [`samples/`](samples/) directory:

| Sample | Description |
|--------|-------------|
| [REST API Sample](samples/FinnhubSdk.Samples.Rest) | Stock quotes, candles, company profiles, news, sentiment |
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
