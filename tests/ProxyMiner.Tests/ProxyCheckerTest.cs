using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Options;
using ProxyMiner.Tests.Moqs;

namespace ProxyMiner.Tests;

[TestClass]
public class ProxyCheckerTest
{
    [TestMethod]
    public void ProxyChecker_InitState()
    {
        var checker = MoqFactory.CheckerBuilder.Build();
        var settings = new Settings { CheckThreadCount = 5 };
        var proxyChecker = new ProxyChecker(checker, settings);
        
        Assert.IsFalse(proxyChecker.IsEnabled);
        Assert.AreEqual(0, proxyChecker.FreeCheckSlot);
    }

    [TestMethod]
    public void ProxyChecker_AddNull()
    {
        var checker = MoqFactory.CheckerBuilder.Build();
        var settings = new Settings { CheckThreadCount = 5 };
        var proxyChecker = new ProxyChecker(checker, settings);
        Assert.ThrowsException<ArgumentNullException>(() => proxyChecker.Add(null!));
    }

    [TestMethod]
    public void ProxyChecker_AddProxiesButNotEnabled()
    {
        var checker = MoqFactory.CheckerBuilder.Build();
        var checkThreadCount = 5;
        var settings = new Settings { CheckThreadCount = checkThreadCount };
        var proxyChecker = new ProxyChecker(checker, settings);
        using var checkObserver = MoqFactory.CheckObserverBuilder.Build();
        proxyChecker.Subscribe(checkObserver);

        proxyChecker.Add(new[] { Proxy1, Proxy2 });
        
        Assert.AreEqual(0, proxyChecker.FreeCheckSlot);
        Assert.IsFalse(proxyChecker.IsEnabled);
    }

    [TestMethod]
    public void ProxyChecker_StopNotStartedSession()
    {
        var checker = MoqFactory.CheckerBuilder.Build();
        var settings = new Settings { CheckThreadCount = 5 };
        var proxyChecker = new ProxyChecker(checker, settings);

        proxyChecker.Stop();
        Assert.AreEqual(0, proxyChecker.FreeCheckSlot);
        Assert.IsFalse(proxyChecker.IsEnabled);
    }

    [TestMethod]
    public void ProxyChecker_StartAndStopWithNoProxies()
    {
        var checker = MoqFactory.CheckerBuilder.Build();
        var checkThreadCount = 5;
        var settings = new Settings { CheckThreadCount = checkThreadCount };
        var proxyChecker = new ProxyChecker(checker, settings);
        using var checkObserver = MoqFactory.CheckObserverBuilder.Build();
        proxyChecker.Subscribe(checkObserver);

        proxyChecker.Start();
        Assert.AreEqual(5, proxyChecker.FreeCheckSlot);
        Assert.IsTrue(proxyChecker.IsEnabled);

        proxyChecker.Stop();
        Assert.AreEqual(0, proxyChecker.FreeCheckSlot);
        Assert.IsFalse(proxyChecker.IsEnabled);
    }

    [TestMethod]
    public void ProxyChecker_AddProxiesThenEnabled()
    {
        var checker = MoqFactory.CheckerBuilder.Build();
        var checkThreadCount = 3;
        var settings = new Settings { CheckThreadCount = checkThreadCount };
        var proxyChecker = new ProxyChecker(checker, settings);
        using var checkObserver = MoqFactory.CheckObserverBuilder.Build();
        proxyChecker.Subscribe(checkObserver);

        proxyChecker.Add(new[] { Proxy1, Proxy2, Proxy3, Proxy4, Proxy5 });
        proxyChecker.Start();
        try
        {
            Assert.IsTrue(proxyChecker.IsEnabled);
            Assert.AreEqual(3, proxyChecker.FreeCheckSlot);
        }
        finally
        {
            proxyChecker.Stop();
        }
    }

    [TestMethod]
    public void ProxyChecker_StartThenAddProxies()
    {
        var checker = MoqFactory.CheckerBuilder.TimeForWork(TimeSpan.FromSeconds(5)).Build();
        var checkThreadCount = 3;
        var settings = new Settings { CheckThreadCount = checkThreadCount };
        var proxyChecker = new ProxyChecker(checker, settings);
        using var checkObserver = MoqFactory.CheckObserverBuilder
            .CheckingFromThese(new[] { Proxy1, Proxy2, Proxy3 })
            .Build();
        proxyChecker.Subscribe(checkObserver);

        proxyChecker.Start();
        try
        {
            Assert.IsTrue(proxyChecker.IsEnabled);
            Assert.AreEqual(3, proxyChecker.FreeCheckSlot);

            proxyChecker.Add(new[] { Proxy1, Proxy2, Proxy3, Proxy4, Proxy5 });
            Thread.Sleep(1000);
            Assert.AreEqual(1, proxyChecker.FreeCheckSlot, "3 proxies in progress, 2 proxies in queue, 1 free slot in queue");
        }
        finally
        {
            proxyChecker.Stop();
        }
    }

    [TestMethod]
    public void ProxyChecker_StopIsAsync()
    {
        var checker = MoqFactory.CheckerBuilder.TimeForWork(TimeSpan.FromSeconds(5)).Build();
        var checkThreadCount = 3;
        var settings = new Settings { CheckThreadCount = checkThreadCount };
        var proxyChecker = new ProxyChecker(checker, settings);
        var tooManyProxies = Enumerable.Range(1, 100).Select(
            x => MakeProxy(ProxyType.Http, "1.1.1.1", 80));
        using var checkObserver = MoqFactory.CheckObserverBuilder
            .CheckingFromThese(tooManyProxies)
            .Build();
        proxyChecker.Subscribe(checkObserver);

        proxyChecker.Start();
        try
        {
            Assert.IsTrue(proxyChecker.IsEnabled);

            proxyChecker.Add(tooManyProxies);
            Thread.Sleep(1000);
        }
        finally
        {
            proxyChecker.Stop();
            Thread.Sleep(1000);
            Assert.AreEqual(0, proxyChecker.FreeCheckSlot);
            Assert.IsFalse(proxyChecker.IsEnabled);
        }
    }

    private static readonly Proxy Proxy1 = MakeProxy(ProxyType.Http, "1.1.1.1", 1111);
    private static readonly Proxy Proxy2 = MakeProxy(ProxyType.Socks4, "2.2.2.2", 2222);
    private static readonly Proxy Proxy3 = MakeProxy(ProxyType.Socks5, "3.3.3.3", 3333);
    private static readonly Proxy Proxy4 = MakeProxy(ProxyType.Http, "4.4.4.4", 4444);
    private static readonly Proxy Proxy5 = MakeProxy(ProxyType.Socks4, "5.5.5.5", 5555);
    private static readonly Proxy Proxy6 = MakeProxy(ProxyType.Socks5, "6.6.6.6", 6666);

    private static Proxy MakeProxy(ProxyType type, string host, int port)
        => Proxy.Factory.TryMakeProxy(type, host, port, out _)!;
}