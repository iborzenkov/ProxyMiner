namespace ProxyMiner.Producers;

public sealed class ProxyMiningEventArgs
{
    public ProxyMiningEventArgs(Producer producer, DateTime startTime)
    {
        Producer = producer;
        StartTime = startTime;
    }

    public Producer Producer { get; }
    public DateTime StartTime { get; }
}