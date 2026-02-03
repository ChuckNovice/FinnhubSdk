using FinnhubSdk.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace FinnhubSdk.Tests.Unit;

[TestClass]
public class CachedFinnhubClientFactoryOptionsTests
{
    [TestMethod]
    public void DefaultOptions_HasSlidingExpiration()
    {
        var options = new CachedFinnhubClientFactoryOptions();

        Assert.IsNotNull(options.EntryOptions.SlidingExpiration);
        Assert.AreEqual(TimeSpan.FromMinutes(30), options.EntryOptions.SlidingExpiration);
    }

    [TestMethod]
    public void DefaultOptions_HasSizeLimit()
    {
        var options = new CachedFinnhubClientFactoryOptions();

        Assert.AreEqual(1000, options.CacheOptions.SizeLimit);
    }

    [TestMethod]
    public void DefaultOptions_HasEntrySize()
    {
        var options = new CachedFinnhubClientFactoryOptions();

        Assert.AreEqual(1, options.EntryOptions.Size);
    }

    [TestMethod]
    public void EntryOptions_CanSetCustomExpiration()
    {
        var options = new CachedFinnhubClientFactoryOptions
        {
            EntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(60),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            }
        };

        Assert.AreEqual(TimeSpan.FromMinutes(60), options.EntryOptions.SlidingExpiration);
        Assert.AreEqual(TimeSpan.FromHours(24), options.EntryOptions.AbsoluteExpirationRelativeToNow);
    }

    [TestMethod]
    public void CacheOptions_CanSetCustomSizeLimit()
    {
        var options = new CachedFinnhubClientFactoryOptions
        {
            CacheOptions = new MemoryCacheOptions
            {
                SizeLimit = 500
            }
        };

        Assert.AreEqual(500, options.CacheOptions.SizeLimit);
    }
}
