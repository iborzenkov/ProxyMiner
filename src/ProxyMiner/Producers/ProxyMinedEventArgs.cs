namespace ProxyMiner.Producers;

public sealed class ProxyMinedEventArgs
{
    public ProxyMinedEventArgs(Producer producer, DateTime finishTime, IEnumerable<Proxy> proxies)
    {
        Producer = producer;
        FinishTime = finishTime;
        Proxies = proxies;
    }

    public Producer Producer { get; }
    public DateTime FinishTime { get; }    
    public IEnumerable<Proxy> Proxies { get; }
}