// ReSharper disable InconsistentNaming

using System.Collections.Concurrent;
using Moq;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Models.BaseCollections;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Tests;

[TestClass]
public class ProducerTests : ProxyMinerTestsBase
{
    [TestMethod]
    public void ProducerCollection_ByDefault_Empty()
    {
        Assert.IsNotNull(Miner.Producers.Items);
        Assert.IsFalse(Miner.Producers.Items.Any());
    }

    [TestMethod]
    public void ProducerCollection_Add_Null()
    {
        Miner.Producers.Add(item: null!);

        Assert.IsFalse(Miner.Producers.Items.Any());
    }

    [TestMethod]
    public void ProducerCollection_AddRange_Null()
    {
        Miner.Producers.AddRange(items: null!);

        Assert.IsFalse(Miner.Producers.Items.Any());
    }

    [TestMethod]
    public void ProducerCollection_Remove_Null()
    {
        Miner.Producers.Remove(item: null!);

        Assert.IsFalse(Miner.Producers.Items.Any());
    }

    [TestMethod]
    public void ProducerCollection_RemoveRange_Null()
    {
        Miner.Producers.RemoveRange(items: null!);

        Assert.IsFalse(Miner.Producers.Items.Any());
    }

    [TestMethod]
    public void ProducerCollection_Add_Remove()
    {
        var producer = new Producer("TestProducer1", GetProvider());

        Miner.Producers.CollectionChanged += Producers_CollectionChanged_Add;
        try
        {
            Miner.Producers.Add(producer);
            CollectionAssert.AreEquivalent(new[] { producer }, Miner.Producers.Items.ToArray());
        }
        finally
        {
            Miner.Producers.CollectionChanged -= Producers_CollectionChanged_Add;
        }

        Miner.Producers.CollectionChanged += Producers_CollectionChanged_Remove;
        try
        {
            Miner.Producers.Remove(producer);

            Assert.IsFalse(Miner.Producers.Items.Any());
        }
        finally
        {
            Miner.Producers.CollectionChanged -= Producers_CollectionChanged_Remove;
        }

        void Producers_CollectionChanged_Add(object? sender, CollectionChangedEventArgs<Producer> e)
        {
            Assert.AreEqual(CollectionChangeAction.Add, e.Action);
            Assert.IsNotNull(e.NewItems);
            CollectionAssert.AreEquivalent(new[] { producer }, e.NewItems.ToArray());
            Assert.IsNull(e.OldItems);
        }

        void Producers_CollectionChanged_Remove(object? sender, CollectionChangedEventArgs<Producer> e)
        {
            Assert.AreEqual(CollectionChangeAction.Remove, e.Action);
            Assert.IsNotNull(e.OldItems);
            CollectionAssert.AreEquivalent(new[] { producer }, e.OldItems.ToArray());
            Assert.IsNull(e.NewItems);
        }
    }

    [TestMethod]
    public void ProducerCollection_AndRange_RemoveRange()
    {
        var producer1 = new Producer("TestProducer1", GetProvider());
        var producer2 = new Producer("TestProducer2", GetProvider());
        var producer3 = new Producer("TestProducer2", GetProvider());

        Miner.Producers.CollectionChanged += Producers_CollectionChanged_Add;
        try
        {
            Miner.Producers.AddRange(new[] { producer1, null!, producer2, producer3, null! });
            CollectionAssert.AreEquivalent(new[] { producer1, producer2, producer3 }, Miner.Producers.Items.ToArray());
        }
        finally
        {
            Miner.Producers.CollectionChanged -= Producers_CollectionChanged_Add;
        }

        Miner.Producers.CollectionChanged += Producers_CollectionChanged_Remove;
        try
        {
            Miner.Producers.RemoveRange(new[] { producer1, producer3, null! });
            CollectionAssert.AreEquivalent(new[] { producer2 }, Miner.Producers.Items.ToArray());
        }
        finally
        {
            Miner.Producers.CollectionChanged -= Producers_CollectionChanged_Remove;
        }

        void Producers_CollectionChanged_Add(object? sender, CollectionChangedEventArgs<Producer> e)
        {
            Assert.AreEqual(CollectionChangeAction.Add, e.Action);
            Assert.IsNotNull(e.NewItems);
            CollectionAssert.AreEquivalent(new[] { producer1, producer2, producer3 }, e.NewItems.ToArray());
            Assert.IsNull(e.OldItems);
        }

        void Producers_CollectionChanged_Remove(object? sender, CollectionChangedEventArgs<Producer> e)
        {
            Assert.AreEqual(CollectionChangeAction.Remove, e.Action);
            Assert.IsNotNull(e.OldItems);
            CollectionAssert.AreEquivalent(new[] { producer1, producer3 }, e.OldItems.ToArray());
            Assert.IsNull(e.NewItems);
        }
    }

    [TestMethod]
    public void ProducerCollection_Start_ProducerEnabled()
    {
        var producer1 = new Producer("TestProducer1", GetProvider()) { IsEnabled = true };
        var producer2 = new Producer("TestProducer2", GetProvider()) { IsEnabled = true };
        var producer3 = new Producer("TestProducer2", GetProvider()) { IsEnabled = true };

        var miningProducers = new ConcurrentBag<Producer>();
        var minedProducers = new ConcurrentBag<Producer>();
        ProxyProviderResultCode? code = null;

        Miner.Producers.Mining += Producers_Mining;
        Miner.Producers.Mined += Producers_Mined;
        try
        {
            Miner.Producers.AddRange(new[] { producer1, producer2, producer3 });
            Miner.Start();
            
            Thread.Sleep(1000);
            CollectionAssert.AreEquivalent(new[] { producer1, producer2, producer3 }, miningProducers);
            CollectionAssert.AreEquivalent(new[] { producer1, producer2, producer3 }, minedProducers);
            Assert.IsTrue(code == ProxyProviderResultCode.Ok);
        }
        finally
        {
            Miner.Producers.Mining -= Producers_Mining;
            Miner.Producers.Mined -= Producers_Mined;
        }

        void Producers_Mining(object? sender, ProxyMiningEventArgs e)
        {
            miningProducers.Add(e.Producer);
        }

        void Producers_Mined(object? sender, ProxyMinedEventArgs e)
        {
            minedProducers.Add(e.Producer);
            code = e.MiningResult.Code;
        }
    }

    [TestMethod]
    public void ProducerCollection_Start_NotEnabled()
    {
        var producer1 = new Producer("TestProducer1", GetProvider());

        var miningProducers = new ConcurrentBag<Producer>();
        var minedProducers = new ConcurrentBag<Producer>();

        Miner.Producers.Mining += Producers_Mining;
        Miner.Producers.Mined += Producers_Mined;
        try
        {
            Miner.Producers.AddRange(new[] { producer1 });
            Miner.Start();

            Thread.Sleep(1000);
            Assert.AreEqual(0, miningProducers.Count);
            Assert.AreEqual(0, minedProducers.Count);
        }
        finally
        {
            Miner.Producers.Mining -= Producers_Mining;
            Miner.Producers.Mined -= Producers_Mined;
        }

        void Producers_Mining(object? sender, ProxyMiningEventArgs e)
        {
            miningProducers.Add(e.Producer);
        }

        void Producers_Mined(object? sender, ProxyMinedEventArgs e)
        {
            minedProducers.Add(e.Producer);
        }
    }

    [TestMethod]
    public void ProducerCollection_Start_NotEnabled_WhenEnable()
    {
        var producer = new Producer("TestProducer1", GetProvider());

        var miningProducers = new ConcurrentBag<Producer>();
        var minedProducers = new ConcurrentBag<Producer>();
        ProxyProviderResultCode? code = null;

        Miner.Producers.Mining += Producers_Mining;
        Miner.Producers.Mined += Producers_Mined;
        try
        {
            Miner.Producers.AddRange(new[] { producer });
            Miner.Start();

            Thread.Sleep(1000);
            Assert.AreEqual(0, miningProducers.Count);
            Assert.AreEqual(0, minedProducers.Count);

            producer.IsEnabled = true;
            Thread.Sleep(1000);
            CollectionAssert.AreEquivalent(new[] { producer }, miningProducers);
            CollectionAssert.AreEquivalent(new[] { producer }, minedProducers);
            Assert.IsTrue(code == ProxyProviderResultCode.Ok);
        }
        finally
        {
            Miner.Producers.Mining -= Producers_Mining;
            Miner.Producers.Mined -= Producers_Mined;
        }

        void Producers_Mining(object? sender, ProxyMiningEventArgs e)
        {
            miningProducers.Add(e.Producer);
        }

        void Producers_Mined(object? sender, ProxyMinedEventArgs e)
        {
            minedProducers.Add(e.Producer);
            code = e.MiningResult.Code;
        }
    }

    [TestMethod]
    public void ProducerCollection_Stop()
    {
        var producer = new Producer("TestProducer1", GetSlowProvider()) { IsEnabled = true };

        var miningProducers = new ConcurrentBag<Producer>();
        var minedProducers = new ConcurrentBag<Producer>();
        ProxyProviderResultCode? code = null;

        Miner.Producers.Mining += Producers_Mining;
        Miner.Producers.Mined += Producers_Mined;
        try
        {
            Miner.Producers.AddRange(new[] { producer });
            Miner.Start();
            Thread.Sleep(1000);

            Miner.Stop();
            Thread.Sleep(1000);
            CollectionAssert.AreEquivalent(new[] { producer }, miningProducers);
            CollectionAssert.AreEquivalent(new[] { producer }, minedProducers);
            Assert.IsTrue(code == ProxyProviderResultCode.Cancelled);
        }
        finally
        {
            Miner.Producers.Mining -= Producers_Mining;
            Miner.Producers.Mined -= Producers_Mined;
        }

        void Producers_Mining(object? sender, ProxyMiningEventArgs e)
        {
            miningProducers.Add(e.Producer);
        }

        void Producers_Mined(object? sender, ProxyMinedEventArgs e)
        {
            minedProducers.Add(e.Producer);
            code = e.MiningResult.Code;
        }
    }

    private IProxyProvider GetSlowProvider()
    {
        var providerMock = new Mock<IProxyProvider>();
        providerMock.Setup(p => p.GetProxies(It.IsAny<CancellationToken>()))
            .Returns(new InvocationFunc(i => Task.Run(() =>
            {
                var token = (CancellationToken)i.Arguments[0];
                token.WaitHandle.WaitOne(5000);
                token.ThrowIfCancellationRequested();
                return ProxyProviderResult.Ok(Enumerable.Empty<Proxy>());
            })));

        return providerMock.Object;
    }

    private IProxyProvider GetProvider()
    {
        var providerMock = new Mock<IProxyProvider>();
        providerMock.Setup(p => p.GetProxies(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(ProxyProviderResult.Ok(Enumerable.Empty<Proxy>())));

        return providerMock.Object;
    }
}