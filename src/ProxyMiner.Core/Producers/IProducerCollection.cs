namespace ProxyMiner.Core.Producers;

public interface IProducerCollection
{
    void Add(Producer producer);

    void Remove(Producer producer);

    IEnumerable<Producer> Items { get; }
    
    event EventHandler<ProxyMiningEventArgs> Mining;
    event EventHandler<ProxyMinedEventArgs> Mined;

    event EventHandler<EventArgs> CollectionChanged;
}