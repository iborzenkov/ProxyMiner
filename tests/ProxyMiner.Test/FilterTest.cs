using ProxyMiner.Filters;

namespace ProxyMiner.Test;

[TestClass]
public class FilterTest
{
    [TestMethod]
    public void Filter_Count()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Count(3).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy1, Proxy2, Proxy3 }, filtered);
    }
    
    [TestMethod]
    public void Filter_Count_CollectionHaveMoreElements()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Count(5).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy1, Proxy2, Proxy3, Proxy4 }, filtered);
    }
    
    [TestMethod]
    public void Filter_Count_CountNotZeroAndCollectionEmpty()
    {
        var proxies = new Dictionary<Proxy, ProxyState>();
        var filter = Filter.Builder.Count(3).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(Array.Empty<Proxy>(), filtered);
    }

    [TestMethod]
    public void Filter_Count_CountZeroAndCollectionNotEmpty()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Count(0).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(Array.Empty<Proxy>(), filtered);
    }
    
    [TestMethod]
    public void Filter_Count_CountNegative()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Count(-1).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(Array.Empty<Proxy>(), filtered);
    }

    private static readonly Proxy Proxy1 = new Proxy(ProxyType.Http, "1.1.1.1", 1111);
    private static readonly Proxy Proxy2 = new Proxy(ProxyType.Http, "2.2.2.2", 2222);
    private static readonly Proxy Proxy3 = new Proxy(ProxyType.Http, "3.3.3.3", 3333);
    private static readonly Proxy Proxy4 = new Proxy(ProxyType.Http, "4.4.4.4", 4444);
    
    private static readonly Dictionary<Proxy, ProxyState> RegularProxis = new()
    {
        { Proxy1, new ProxyState(DateTime.Now, DateTime.Now, ProxyStatus.Anonimous) },
        { Proxy2, new ProxyState(DateTime.Now, DateTime.Now, ProxyStatus.Cancelled) },
        { Proxy3, new ProxyState(DateTime.Now, DateTime.Now, ProxyStatus.NotAnonimous) },
        { Proxy4, new ProxyState(DateTime.Now, DateTime.Now, ProxyStatus.Anonimous) },
    };
}