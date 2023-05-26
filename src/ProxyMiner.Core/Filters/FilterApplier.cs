using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Filters;

/// <summary>
///     Gets an indication of the filter and applies it to the proxies collection.
/// </summary>
internal sealed class FilterApplier
{
    public FilterApplier(Dictionary<Proxy, ProxyState> proxyWithState)
    {
        _proxyWithState = proxyWithState;
    }
    
    public List<Proxy> Apply(Filter filter)
    {
        var allProxies = _proxyWithState.Select(p => p.Key).ToList();
        var withoutExcluded = allProxies.Except(filter.ExcludedProxies).ToList();
        var valided = GetValidProxies(withoutExcluded, filter.IsValid);
        var anonimous = GetAnonimousProxies(valided, filter.IsAnonimous);
        var withNotActualStates = GetProxiesWithNotActualStates(anonimous, filter.ExpiredState);
        var sorted = SortIfNeed(withNotActualStates, filter.Sort);
        sorted.InsertRange(0, filter.IncludedProxies.Except(filter.ExcludedProxies));
        return filter.Count == null
            ? sorted
            : sorted.Take(filter.Count ?? sorted.Count).ToList();

    }
    
    private List<Proxy> SortIfNeed(List<Proxy> proxies, ProxySort? sort)
    {
        if (sort != null)
        {
            proxies.Sort(ProxyComparerFactory.Make(sort, proxy => _proxyWithState[proxy]));
        }

        return proxies;
    }

    private List<Proxy> GetProxiesWithNotActualStates(List<Proxy> proxies, TimeSpan? expiredState)
    {
        if (expiredState == null)
            return proxies;

        var result = new List<Proxy>();

        foreach (var proxy in proxies)
        {
            var state = _proxyWithState[proxy];
            if (state.FinishTimeUtc == null)
            {
                result.Add(proxy);
            }
            else
            {
                if (state.FinishTimeUtc.Value + expiredState.Value <= DateTime.UtcNow)
                    result.Add(proxy);
            }
        }

        return result;
    }
    
    private List<Proxy> GetValidProxies(List<Proxy> proxies, bool? isValid)
    {
        if (isValid == null)
            return proxies;

        var result = new List<Proxy>();
        foreach (var proxy in proxies)
        {
            var state = _proxyWithState[proxy];
            if (state.Status == null)
                continue;

            if (state.Status.IsValid && isValid.Value || !state.Status.IsValid && !isValid.Value)
                result.Add(proxy);
        }

        return result;
    }
    
    private List<Proxy> GetAnonimousProxies(List<Proxy> proxies, bool? isAnonimous)
    {
        if (isAnonimous == null)
            return proxies;

        var result = new List<Proxy>();
        foreach (var proxy in proxies)
        {
            var state = _proxyWithState[proxy];
            if (state.Status == null)
                continue;

            if (state.Status.IsAnonimous && isAnonimous.Value || !state.Status.IsAnonimous && !isAnonimous.Value)
                result.Add(proxy);
        }

        return result;
    }

    private readonly Dictionary<Proxy, ProxyState> _proxyWithState;
}