namespace FinnhubSdk.Tests.Unit.WebSocket;

using System.Text.Json;
using FinnhubSdk.WebSocket.Models;

[TestClass]
public class WebSocketModelsTests
{
    [TestMethod]
    public void Trade_Deserialize_CorrectlyMapsProperties()
    {
        // Arrange
        var json = """
            {
                "s": "AAPL",
                "p": 150.25,
                "v": 100,
                "t": 1234567890123,
                "c": ["@", "T"]
            }
            """;

        // Act
        var trade = JsonSerializer.Deserialize<Trade>(json);

        // Assert
        Assert.IsNotNull(trade);
        Assert.AreEqual("AAPL", trade.Symbol);
        Assert.AreEqual(150.25m, trade.Price);
        Assert.AreEqual(100m, trade.Volume);
        Assert.AreEqual(1234567890123L, trade.Timestamp);
        Assert.IsNotNull(trade.Conditions);
        Assert.AreEqual(2, trade.Conditions.Length);
        Assert.AreEqual("@", trade.Conditions[0]);
        Assert.AreEqual("T", trade.Conditions[1]);
    }

    [TestMethod]
    public void Trade_Deserialize_WithoutConditions_ReturnsNullConditions()
    {
        // Arrange
        var json = """
            {
                "s": "MSFT",
                "p": 350.50,
                "v": 50,
                "t": 1234567890000
            }
            """;

        // Act
        var trade = JsonSerializer.Deserialize<Trade>(json);

        // Assert
        Assert.IsNotNull(trade);
        Assert.AreEqual("MSFT", trade.Symbol);
        Assert.IsNull(trade.Conditions);
    }

    [TestMethod]
    public void TradeMessage_Deserialize_CorrectlyMapsProperties()
    {
        // Arrange
        var json = """
            {
                "type": "trade",
                "data": [
                    {"s": "AAPL", "p": 150.25, "v": 100, "t": 1234567890123},
                    {"s": "AAPL", "p": 150.30, "v": 50, "t": 1234567890125}
                ]
            }
            """;

        // Act
        var message = JsonSerializer.Deserialize<TradeMessage>(json);

        // Assert
        Assert.IsNotNull(message);
        Assert.AreEqual("trade", message.Type);
        Assert.AreEqual(2, message.Data.Length);
        Assert.AreEqual("AAPL", message.Data[0].Symbol);
        Assert.AreEqual(150.25m, message.Data[0].Price);
        Assert.AreEqual(150.30m, message.Data[1].Price);
    }

    [TestMethod]
    public void TradeMessage_Deserialize_EmptyData_ReturnsEmptyArray()
    {
        // Arrange
        var json = """
            {
                "type": "trade",
                "data": []
            }
            """;

        // Act
        var message = JsonSerializer.Deserialize<TradeMessage>(json);

        // Assert
        Assert.IsNotNull(message);
        Assert.AreEqual("trade", message.Type);
        Assert.AreEqual(0, message.Data.Length);
    }

    [TestMethod]
    public void WebSocketMessage_Subscribe_CreatesCorrectMessage()
    {
        // Act
        var message = WebSocketMessage.Subscribe("AAPL");

        // Assert
        Assert.AreEqual("subscribe", message.Type);
        Assert.AreEqual("AAPL", message.Symbol);
    }

    [TestMethod]
    public void WebSocketMessage_Unsubscribe_CreatesCorrectMessage()
    {
        // Act
        var message = WebSocketMessage.Unsubscribe("AAPL");

        // Assert
        Assert.AreEqual("unsubscribe", message.Type);
        Assert.AreEqual("AAPL", message.Symbol);
    }

    [TestMethod]
    public void WebSocketMessage_Subscribe_SerializesToCorrectJson()
    {
        // Arrange
        var message = WebSocketMessage.Subscribe("BINANCE:BTCUSDT");

        // Act
        var json = JsonSerializer.Serialize(message);

        // Assert
        Assert.IsTrue(json.Contains("\"type\":\"subscribe\""));
        Assert.IsTrue(json.Contains("\"symbol\":\"BINANCE:BTCUSDT\""));
    }
}
