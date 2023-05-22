using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Checkers;

public sealed class ProxyCheckedEventArgs : EventArgs
{
    public ProxyCheckedEventArgs(Proxy proxy, ProxyState state)
    {
        Proxy = proxy;
        State = state;
    }

    public Proxy Proxy { get; }
    public ProxyState State { get; }
}