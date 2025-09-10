using System.Collections.Concurrent;
using ProxyMiner.Core.Models.BaseCollections;
using ProxyMiner.Core.Producers;
using ProxyMiner.Tests.Moqs;

namespace ProxyMiner.Tests;

[TestClass]
public class ProducerCollectionTests : ProxyMinerTestsBase
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
        Assert.ThrowsException<ArgumentNullException>(
            () => Miner.Producers.Add(item: null!));
    }

    [TestMethod]
    public void ProducerCollection_AddRange_Null()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => Miner.Producers.AddRange(items: null!));
    }

    [TestMethod]
    public void ProducerCollection_Remove_Null()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => Miner.Producers.Remove(item: null!));
    }

    [TestMethod]
    public void ProducerCollection_RemoveRange_Null()
    {
        Assert.ThrowsException<ArgumentNullException>(
            () => Miner.Producers.RemoveRange(items: null!));
    }

    [TestMethod]
    public void ProducerCollection_Add_Remove()
    {
        var producer = MakeSimpleProducer();
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
        var producer1 = MakeSimpleProducer("TestProducer1");
        var producer2 = MakeSimpleProducer("TestProducer2");
        var producer3 = MakeSimpleProducer("TestProducer2");

        Miner.Producers.CollectionChanged += Producers_CollectionChanged_Add;
        try
        {
            Miner.Producers.AddRange([producer1, null!, producer2, producer3, null!]);
            CollectionAssert.AreEquivalent(new[] { producer1, producer2, producer3 }, Miner.Producers.Items.ToArray());
        }
        finally
        {
            Miner.Producers.CollectionChanged -= Producers_CollectionChanged_Add;
        }

        Miner.Producers.CollectionChanged += Producers_CollectionChanged_Remove;
        try
        {
            Miner.Producers.RemoveRange([producer1, producer3, null!]);
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
        var producer1 = MakeSimpleProducer("TestProducer1");
        producer1.IsEnabled = true;
        var producer2 = MakeSimpleProducer("TestProducer2");
        producer2.IsEnabled = true;
        var producer3 = MakeSimpleProducer("TestProducer2");
        producer3.IsEnabled = true;

        var miningProducers = new ConcurrentBag<Producer>();
        var minedProducers = new ConcurrentBag<Producer>();
        ProxyProviderResultCode? code = null;

        Miner.Producers.Mining += Producers_Mining;
        Miner.Producers.Mined += Producers_Mined;
        try
        {
            Miner.Producers.AddRange([producer1, producer2, producer3]);
            StartMiner();

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
        var miningProducers = new ConcurrentBag<Producer>();
        var minedProducers = new ConcurrentBag<Producer>();

        Miner.Producers.Mining += Producers_Mining;
        Miner.Producers.Mined += Producers_Mined;
        try
        {
            Miner.Producers.AddRange([MakeSimpleProducer()]);
            StartMiner();

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
    public void ProducerCollection_Start_NotEnabled_ThenEnable()
    {
        var producer = MakeSimpleProducer();

        var miningProducers = new ConcurrentBag<Producer>();
        var minedProducers = new ConcurrentBag<Producer>();
        ProxyProviderResultCode? code = null;

        Miner.Producers.Mining += Producers_Mining;
        Miner.Producers.Mined += Producers_Mined;
        try
        {
            Miner.Producers.AddRange([producer]);
            StartMiner();

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
        var producer = new Producer(
            "TestProducer", 
            MoqFactory.ProxyProviderBuilder
                .ReturnOk()
                .TimeForWork(TimeSpan.FromSeconds(5))
                .Build()) 
        { 
            IsEnabled = true 
        };

        var miningProducers = new ConcurrentBag<Producer>();
        var minedProducers = new ConcurrentBag<Producer>();
        ProxyProviderResultCode? code = null;

        Miner.Producers.Mining += Producers_Mining;
        Miner.Producers.Mined += Producers_Mined;
        try
        {
            Miner.Producers.AddRange([producer]);
            StartMiner();
            StopMiner();

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

    private static Producer MakeSimpleProducer(string? name = null) =>
        new(name ?? "TestProducer", MoqFactory.ProxyProviderBuilder.ReturnOk().Build());
}