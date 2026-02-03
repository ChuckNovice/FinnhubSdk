namespace FinnhubSdk.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public class CachedFactoryIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task GetOrCreateClient_CanGetCompanyProfile()
    {
        await RateLimitDelayAsync();

        var client = CachedFactory.GetOrCreateClient(ApiKey);
        var profile = await client.Stocks.GetCompanyProfileAsync("AAPL");

        Assert.IsNotNull(profile);
        Assert.AreEqual("AAPL", profile.Ticker);
    }

    [TestMethod]
    public async Task GetOrCreateClient_CanGetQuote()
    {
        await RateLimitDelayAsync();

        var client = CachedFactory.GetOrCreateClient(ApiKey);
        var quote = await client.Stocks.GetQuoteAsync("TSLA");

        Assert.IsNotNull(quote);
        Assert.IsTrue(quote.CurrentPrice > 0);
    }

    [TestMethod]
    public async Task GetOrCreateClient_SameKeyReturnsCachedClient_BothCanCallApi()
    {
        var client1 = CachedFactory.GetOrCreateClient(ApiKey);
        var client2 = CachedFactory.GetOrCreateClient(ApiKey);

        Assert.AreSame(client1, client2);

        await RateLimitDelayAsync();
        var profile = await client1.Stocks.GetCompanyProfileAsync("NVDA");
        Assert.IsNotNull(profile);
    }

    [TestMethod]
    public async Task RemoveAsync_ThenGetOrCreate_GetsNewClientThatCanCallApi()
    {
        var client1 = CachedFactory.GetOrCreateClient(ApiKey);
        await CachedFactory.RemoveAsync(ApiKey);
        var client2 = CachedFactory.GetOrCreateClient(ApiKey);

        Assert.AreNotSame(client1, client2);

        await RateLimitDelayAsync();
        var profile = await client2.Stocks.GetCompanyProfileAsync("META");
        Assert.IsNotNull(profile);
    }
}
