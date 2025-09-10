using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Filters;

/// <summary>
///     Filter builder.
/// </summary>
public sealed class FilterBuilder
{
    public FilterBuilder Count(int value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(nameof(Count), $"The Count must be greater than 0");

        _filter.Count = value;
        return this;
    }

    public FilterBuilder Include(IEnumerable<Proxy> proxies)
    {
        ArgumentNullException.ThrowIfNull(proxies);

        _filter.IncludedProxies.AddRange(proxies);
        return this;
    }

    public FilterBuilder Except(IEnumerable<Proxy> proxies)
    {
        ArgumentNullException.ThrowIfNull(proxies);

        _filter.ExcludedProxies.AddRange(proxies);
        return this;
    }

    public FilterBuilder StateNotExpired(TimeSpan expiredState)
    {
        _filter.ExpiredState = expiredState;
        return this;
    }

    public FilterBuilder SortedBy(SortingField field, SortDirection direction)
    {
        _filter.Sort = new ProxySort(field, direction);
        return this;
    }

    public FilterBuilder Valid(bool isValid)
    {
        _filter.IsValid = isValid;
        return this;
    }

    public FilterBuilder Anonimous(bool isAnonimous)
    {
        _filter.IsAnonimous = isAnonimous;
        return this;
    }

    public Filter Build() => _filter;

    private readonly Filter _filter = new();
}