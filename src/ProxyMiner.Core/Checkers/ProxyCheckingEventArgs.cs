using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Checkers;

/// <summary>
///     Arguments of the event about the beginning of the proxy check.
/// </summary>
public sealed class ProxyCheckingEventArgs : EventArgs
{
    public ProxyCheckingEventArgs(Proxy proxy, DateTime startTimeUtc)
    {
        Proxy = proxy;
        StartTimeUtc = startTimeUtc;
    }

    /// <summary>
    ///     Proxy.
    /// </summary>
    public Proxy Proxy { get; }

    /// <summary>
    ///     Check start time.
    /// </summary>
    public DateTime StartTimeUtc { get; }
}