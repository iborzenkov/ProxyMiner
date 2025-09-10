using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Filters;

/// <summary>
///     Filter for the proxy collection.
/// </summary>
public sealed class Filter
{
    public Filter() 
    { }

    /// <summary>
    ///     Number of items in the resulting collection.
    /// </summary>
    public int? Count { get; internal set; }

    /// <summary>
    ///     Proxies that should not be in the resulting collection.
    /// </summary>
    public List<Proxy> ExcludedProxies { get; } = [];

    /// <summary>
    ///     Proxies that should be in the resulting collection.
    /// </summary>
    public List<Proxy> IncludedProxies { get; } = [];

    /// <summary>
    ///     Whether to sort the resulting collection.
    /// </summary>
    public ProxySort? Sort { get; internal set; }

    /// <summary>
    ///     Indicates that the resulting collection should only have valid/invalid proxies.
    /// </summary>
    public bool? IsValid { get; internal set; }

    /// <summary>
    ///     Indicates that the resulting collection should only have anonimous (not anonimous) proxies.
    /// </summary>
    public bool? IsAnonimous { get; internal set; }

    /// <summary>
    ///     The time since the last proxy check.
    /// </summary>
    /// <remarks>If the time has not expired yet, do not include the proxy in the resulting collection.</remarks>
    public TimeSpan? ExpiredState { get; internal set; }
}