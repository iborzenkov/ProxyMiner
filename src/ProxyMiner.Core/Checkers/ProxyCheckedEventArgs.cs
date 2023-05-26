using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Checkers;

/// <summary>
///     Arguments of the event about the end of the proxy check.
/// </summary>
public sealed class ProxyCheckedEventArgs : EventArgs
{
    public ProxyCheckedEventArgs(Proxy proxy, ProxyState state)
    {
        Proxy = proxy;
        State = state;
    }

    /// <summary>
    ///     Proxy.
    /// </summary>
    public Proxy Proxy { get; }

    /// <summary>
    ///     Proxy state after verification.
    /// </summary>
    public ProxyState State { get; }
}