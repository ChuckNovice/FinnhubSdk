namespace FinnhubSdk.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public class StatelessFactoryIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task CreateClient_CanGetCompanyProfile()
    {
        await RateLimitDelayAsync();

        var client = Factory.CreateClient(ApiKey);
        var profile = await client.Stocks.GetCompanyProfileAsync("AAPL");

        Assert.IsNotNull(profile);
        Assert.AreEqual("AAPL", profile.Ticker);
        Assert.IsFalse(string.IsNullOrEmpty(profile.Name));
    }

    [TestMethod]
    public async Task CreateClient_CanGetQuote()
    {
        await RateLimitDelayAsync();

        var client = Factory.CreateClient(ApiKey);
        var quote = await client.Stocks.GetQuoteAsync("MSFT");

        Assert.IsNotNull(quote);
        Assert.IsTrue(quote.CurrentPrice > 0);
    }

    [TestMethod]
    public async Task CreateClient_CanGetMarketNews()
    {
        await RateLimitDelayAsync();

        var client = Factory.CreateClient(ApiKey);
        var news = await client.News.GetMarketNewsAsync("general");

        Assert.IsNotNull(news);
        Assert.IsTrue(news.Count > 0);
    }

    [TestMethod]
    public async Task CreateClient_MultipleClients_EachCanCallApi()
    {
        await RateLimitDelayAsync();
        var client1 = Factory.CreateClient(ApiKey);
        var profile1 = await client1.Stocks.GetCompanyProfileAsync("AAPL");

        await RateLimitDelayAsync();
        var client2 = Factory.CreateClient(ApiKey);
        var profile2 = await client2.Stocks.GetCompanyProfileAsync("GOOGL");

        Assert.IsNotNull(profile1);
        Assert.IsNotNull(profile2);
        Assert.AreNotSame(client1, client2);
    }
}
