namespace ProxyMiner.Core.Models;

/// <summary>
///     State of proxy.
/// </summary>
public sealed record StateOfProxy
{
    internal StateOfProxy(Proxy proxy, ProxyState state) 
    {
        Proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
        State = state ?? throw new ArgumentNullException(nameof(state));
    }

    /// <summary>
    ///     Proxy.
    /// </summary>
    public Proxy Proxy { get; }

    /// <summary>
    ///     Proxy state.
    /// </summary>
    public ProxyState State{ get; }
}