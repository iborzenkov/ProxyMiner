namespace ProxyMiner;

public sealed class ProxyCheckingEventArgs : EventArgs
{
    public ProxyCheckingEventArgs(Proxy proxy, DateTime startTime)
    {
        Proxy = proxy;
        StartTime = startTime;
    }

    public Proxy Proxy { get; }
    public DateTime StartTime { get; }
}