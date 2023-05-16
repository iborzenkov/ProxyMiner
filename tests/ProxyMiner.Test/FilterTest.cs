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

    [TestMethod]
    public void Filter_IsValid_True()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Valid(true).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy1, Proxy3, Proxy4 }, filtered);
    }

    [TestMethod]
    public void Filter_IsValid_False()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Valid(false).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy2 }, filtered);
    }

    [TestMethod]
    public void Filter_Anonimous_True()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Anonimous(true).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy1, Proxy4 }, filtered);
    }

    [TestMethod]
    public void Filter_Anonimous_False()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Anonimous(false).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy2, Proxy3 }, filtered);
    }

    [TestMethod]
    public void Filter_Except()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Except(new [] { Proxy1, Proxy3 } ).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy2, Proxy4 }, filtered);
    }

    [TestMethod]
    public void Filter_Except_All()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Except(new [] { Proxy1, Proxy2, Proxy3, Proxy4 } ).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(Array.Empty<object>(), filtered);
    }

    [TestMethod]
    public void Filter_Include()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.Include(new [] { Proxy5 } ).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy5, Proxy1, Proxy2, Proxy3, Proxy4 }, filtered);
    }

    [TestMethod]
    public void Filter_Sort_ByLastCheck_Asceding()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.SortedBy(SortingField.LastCheck, SortDirection.Asceding ).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy4, Proxy2, Proxy1, Proxy3 }, filtered);
    }

    [TestMethod]
    [Ignore] // todo: implement
    public void Filter_Sort_ByLastCheck_Descending()
    {
        var proxies = RegularProxis;
        var filter = Filter.Builder.SortedBy(SortingField.LastCheck, SortDirection.Descending ).Build();
        
        var filtered = new FilterApplier(proxies).Apply(filter);

        CollectionAssert.AreEqual(new[] { Proxy3, Proxy1, Proxy2, Proxy4 }, filtered);
    }

    private static readonly Proxy Proxy1 = new Proxy(ProxyType.Http, "1.1.1.1", 1111);
    private static readonly Proxy Proxy2 = new Proxy(ProxyType.Socks4, "2.2.2.2", 2222);
    private static readonly Proxy Proxy3 = new Proxy(ProxyType.Socks5, "3.3.3.3", 3333);
    private static readonly Proxy Proxy4 = new Proxy(ProxyType.Http, "4.4.4.4", 4444);
    private static readonly Proxy Proxy5 = new Proxy(ProxyType.Socks4, "5.5.5.5", 5555);
    
    private static readonly Dictionary<Proxy, ProxyState> RegularProxis = new()
    {
        { Proxy1, new ProxyState(new DateTime(2023, 1, 25), new DateTime(2023, 1, 26), ProxyStatus.Anonimous) },
        { Proxy2, new ProxyState(new DateTime(2023, 1, 23), new DateTime(2023, 1, 24), ProxyStatus.Cancelled) },
        { Proxy3, new ProxyState(new DateTime(2023, 1, 27), new DateTime(2023, 1, 28), ProxyStatus.NotAnonimous) },
        { Proxy4, new ProxyState(new DateTime(2023, 1, 21), new DateTime(2023, 1, 22), ProxyStatus.Anonimous) },
    };
}