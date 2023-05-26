namespace ProxyMiner.Core.Producers;

public sealed class ProxyMiningEventArgs
{
    public ProxyMiningEventArgs(Producer producer, DateTime startTimeUtc)
    {
        Producer = producer;
        StartTimeUtc = startTimeUtc;
    }

    public Producer Producer { get; }
    public DateTime StartTimeUtc { get; }
}