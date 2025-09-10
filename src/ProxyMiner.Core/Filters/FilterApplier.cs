using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Filters;

/// <summary>
///     Gets an indication of the filter and applies it to the proxies collection.
/// </summary>
internal sealed class FilterApplier
{
    internal FilterApplier(IEnumerable<StateOfProxy> proxyStates)
    {
        _proxyStates = proxyStates.ToHashSet() ?? throw new ArgumentNullException(nameof(proxyStates));
    }
    
    public List<Proxy> Apply(Filter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var allProxies = _proxyStates.ToList();
        var withoutExcluded = allProxies.Except(
            _proxyStates.Where(ps => filter.ExcludedProxies.Contains(ps.Proxy))).ToList();
        var valided = GetValidProxies(withoutExcluded, filter.IsValid);
        var anonimous = GetAnonimousProxies(valided, filter.IsAnonimous);
        var withNotActualStates = GetProxiesWithNotActualStates(anonimous, filter.ExpiredState);
        var sorted = SortIfNeed(withNotActualStates, filter.Sort);
        
        var proxies = sorted.Select(ps => ps.Proxy).ToList();
        proxies.InsertRange(0, filter.IncludedProxies.Except(filter.ExcludedProxies));
        return filter.Count == null
            ? proxies
            : proxies.Take(filter.Count ?? proxies.Count).ToList();
    }
    
    private static List<StateOfProxy> SortIfNeed(List<StateOfProxy> proxies, ProxySort? sort)
    {
        if (sort != null)
        {
            proxies.Sort(ProxyComparerFactory.Make(sort));
        }

        return proxies;
    }

    private static List<StateOfProxy> GetProxiesWithNotActualStates(List<StateOfProxy> proxies, TimeSpan? expiredState)
    {
        if (expiredState == null)
            return proxies;

        var result = new List<StateOfProxy>();

        foreach (var proxy in proxies)
        {
            if (proxy.State.FinishTimeUtc == null 
                || (proxy.State.Status != null && proxy.State.Status.IsCancelled))
            {
                result.Add(proxy);
            }
            else
            {
                if (proxy.State.FinishTimeUtc.Value + expiredState.Value <= DateTime.UtcNow)
                    result.Add(proxy);
            }
        }

        return result;
    }
    
    private static List<StateOfProxy> GetValidProxies(List<StateOfProxy> proxies, bool? isValid)
    {
        if (isValid == null)
            return proxies;

        var result = new List<StateOfProxy>();
        foreach (var proxy in proxies)
        {
            if (proxy.State.Status == null)
                continue;

            if (proxy.State.Status.IsValid && isValid.Value 
                || !proxy.State.Status.IsValid && !isValid.Value)
                result.Add(proxy);
        }

        return result;
    }
    
    private static List<StateOfProxy> GetAnonimousProxies(List<StateOfProxy> proxies, bool? isAnonimous)
    {
        if (isAnonimous == null)
            return proxies;

        var result = new List<StateOfProxy>();
        foreach (var proxy in proxies)
        {
            if (proxy.State.Status == null)
                continue;

            if (proxy.State.Status.IsAnonimous && isAnonimous.Value 
                || !proxy.State.Status.IsAnonimous && !isAnonimous.Value)
                result.Add(proxy);
        }

        return result;
    }

    private readonly HashSet<StateOfProxy> _proxyStates;
}