using ProxyMiner.Core.Models;
using ProxyMiner.Core.Models.BaseCollections;

namespace ProxyMiner.Tests;

[TestClass]
public class ProxyCollectionTests : ProxyMinerTestsBase
{
    [TestMethod]
    public void ProxyCollection_ByDefault_Empty()
    {
        Assert.IsNotNull(Miner.Proxies.Items);
        Assert.IsFalse(Miner.Proxies.Items.Any());
    }

    [TestMethod]
    public void ProxyCollection_Add_Null()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => Miner.Proxies.Add(item: null!));
    }

    [TestMethod]
    public void ProxyCollection_AddRange_Null()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => Miner.Proxies.AddRange(items: null!));
    }

    [TestMethod]
    public void ProxyCollection_Remove_Null()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => Miner.Proxies.Remove(item: null!));
    }

    [TestMethod]
    public void ProxyCollection_RemoveRange_Null()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => Miner.Proxies.RemoveRange(items: null!));
    }

    [TestMethod]
    public void ProxyCollection_Add_Remove()
    {
        var proxy = Proxy.Factory.TryMakeProxy(ProxyType.Http, "127.0.0.1", 80, out _);
        Assert.IsNotNull(proxy);

        Miner.Proxies.CollectionChanged += Proxies_CollectionChanged_Add;
        try
        {
            Miner.Proxies.Add(proxy);
            CollectionAssert.AreEquivalent(new[] { proxy }, Miner.Proxies.Items.ToArray());
        }
        finally
        {
            Miner.Proxies.CollectionChanged -= Proxies_CollectionChanged_Add;
        }

        Miner.Proxies.CollectionChanged += Proxies_CollectionChanged_Remove;
        try
        {
            Miner.Proxies.Remove(proxy);

            Assert.IsFalse(Miner.Proxies.Items.Any());
        }
        finally
        {
            Miner.Proxies.CollectionChanged -= Proxies_CollectionChanged_Remove;
        }

        void Proxies_CollectionChanged_Add(object? sender, CollectionChangedEventArgs<Proxy> e)
        {
            Assert.AreEqual(CollectionChangeAction.Add, e.Action);
            Assert.IsNotNull(e.NewItems);
            CollectionAssert.AreEquivalent(new[] { proxy }, e.NewItems.ToArray());
            Assert.IsNull(e.OldItems);
        }

        void Proxies_CollectionChanged_Remove(object? sender, CollectionChangedEventArgs<Proxy> e)
        {
            Assert.AreEqual(CollectionChangeAction.Remove, e.Action);
            Assert.IsNotNull(e.OldItems);
            CollectionAssert.AreEquivalent(new[] { proxy }, e.OldItems.ToArray());
            Assert.IsNull(e.NewItems);
        }
    }

    [TestMethod]
    public void ProxyCollection_AndRange_RemoveRange()
    {
        var proxy1 = Proxy.Factory.TryMakeProxy(ProxyType.Http, "127.0.0.1", 80, out _);
        Assert.IsNotNull(proxy1);
        var proxy2 = Proxy.Factory.TryMakeProxy(ProxyType.Socks4, "127.0.0.2", 81, out _);
        Assert.IsNotNull(proxy2);
        var proxy3 = Proxy.Factory.TryMakeProxy(ProxyType.Socks5, "127.0.0.3", 82, out _);
        Assert.IsNotNull(proxy3);

        Miner.Proxies.CollectionChanged += Proxies_CollectionChanged_Add;
        try
        {
            Miner.Proxies.AddRange([proxy1, null, proxy2, proxy3, null]);
            CollectionAssert.AreEquivalent(new[] { proxy1, proxy2, proxy3 }, Miner.Proxies.Items.ToArray());
        }
        finally
        {
            Miner.Proxies.CollectionChanged -= Proxies_CollectionChanged_Add;
        }

        Miner.Proxies.CollectionChanged += Proxies_CollectionChanged_Remove;
        try
        {
            Miner.Proxies.RemoveRange([proxy1, proxy3, null]);
            CollectionAssert.AreEquivalent(new[] { proxy2 }, Miner.Proxies.Items.ToArray());
        }
        finally
        {
            Miner.Proxies.CollectionChanged -= Proxies_CollectionChanged_Remove;
        }

        void Proxies_CollectionChanged_Add(object? sender, CollectionChangedEventArgs<Proxy> e)
        {
            Assert.AreEqual(CollectionChangeAction.Add, e.Action);
            Assert.IsNotNull(e.NewItems);
            CollectionAssert.AreEquivalent(new[] { proxy1, proxy2, proxy3 }, e.NewItems.ToArray());
            Assert.IsNull(e.OldItems);
        }

        void Proxies_CollectionChanged_Remove(object? sender, CollectionChangedEventArgs<Proxy> e)
        {
            Assert.AreEqual(CollectionChangeAction.Remove, e.Action);
            Assert.IsNotNull(e.OldItems);
            CollectionAssert.AreEquivalent(new[] { proxy1, proxy3 }, e.OldItems.ToArray());
            Assert.IsNull(e.NewItems);
        }
    }
}