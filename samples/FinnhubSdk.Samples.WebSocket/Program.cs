using FinnhubSdk.Clients;
using FinnhubSdk.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Build host with Finnhub SDK
var builder = Host.CreateApplicationBuilder(args);

// Configure from environment variable or appsettings.json
var apiKey = Environment.GetEnvironmentVariable("FINNHUB_API_KEY")
    ?? builder.Configuration["Finnhub:ApiKey"]
    ?? throw new InvalidOperationException(
        "API key not found. Set FINNHUB_API_KEY environment variable or configure in appsettings.json");

builder.Services.AddFinnhub(options => options.ApiKey = apiKey);

var host = builder.Build();

// Get the Finnhub client
var finnhub = host.Services.GetRequiredService<IFinnhubClient>();
var wsClient = finnhub.WebSocket;

Console.WriteLine("=== FinnhubSdk WebSocket Streaming Sample ===");
Console.WriteLine();

// Set up callbacks
wsClient.OnConnectionStateChanged = state =>
{
    Console.WriteLine($"[Connection] State changed to: {state}");
    return Task.CompletedTask;
};

wsClient.OnTradeReceived = message =>
{
    foreach (var trade in message.Data)
    {
        Console.WriteLine($"[Trade] {trade.Symbol} @ ${trade.Price:F4} | Volume: {trade.Volume} | Time: {trade.Timestamp:HH:mm:ss.fff}");
    }
    return Task.CompletedTask;
};

wsClient.OnErrorOccurred = ex =>
{
    Console.WriteLine($"[Error] {ex.GetType().Name}: {ex.Message}");
    return Task.CompletedTask;
};

// Set up cancellation for Ctrl+C
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    Console.WriteLine();
    Console.WriteLine("Shutting down...");
    e.Cancel = true;
    cts.Cancel();
};

try
{
    // Connect to WebSocket
    Console.WriteLine("Connecting to Finnhub WebSocket...");
    await wsClient.ConnectAsync(cts.Token);

    // Subscribe to symbols
    var symbols = new[] { "AAPL", "MSFT", "GOOGL" };
    Console.WriteLine($"Subscribing to: {string.Join(", ", symbols)}");
    await wsClient.SubscribeAsync(symbols, cts.Token);

    Console.WriteLine();
    Console.WriteLine("Listening for real-time trades. Press Ctrl+C to exit.");
    Console.WriteLine("Note: Trades only occur during market hours (9:30 AM - 4:00 PM ET, Mon-Fri)");
    Console.WriteLine();

    // Wait for cancellation
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (OperationCanceledException)
{
    // Expected when Ctrl+C is pressed
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// Clean up
Console.WriteLine("Unsubscribing from all symbols...");
await wsClient.UnsubscribeAllAsync();

Console.WriteLine("Disconnecting...");
await wsClient.DisconnectAsync();

Console.WriteLine();
Console.WriteLine("=== Sample Complete ===");
