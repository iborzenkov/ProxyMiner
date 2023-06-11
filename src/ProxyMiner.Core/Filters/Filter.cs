using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Filters;

/// <summary>
///     Filter for the proxy collection.
/// </summary>
public sealed class Filter
{
    private Filter() 
    { }

    /// <summary>
    ///     Number of items in the resulting collection.
    /// </summary>
    public int? Count { get; private init; }

    /// <summary>
    ///     Proxies that should not be in the resulting collection.
    /// </summary>
    public IEnumerable<Proxy> ExcludedProxies { get; private init; } = Enumerable.Empty<Proxy>();

    /// <summary>
    ///     Proxies that should be in the resulting collection.
    /// </summary>
    public IEnumerable<Proxy> IncludedProxies { get; private init; } = Enumerable.Empty<Proxy>();

    /// <summary>
    ///     Whether to sort the resulting collection.
    /// </summary>
    public ProxySort? Sort { get; private init; }

    /// <summary>
    ///     Indicates that the resulting collection should only have valid/invalid proxies.
    /// </summary>
    public bool? IsValid { get; private init; }

    /// <summary>
    ///     Indicates that the resulting collection should only have anonimous (not anonimous) proxies.
    /// </summary>
    public bool? IsAnonimous { get; private init; }

    /// <summary>
    ///     The time since the last proxy check.
    /// </summary>
    /// <remarks>If the time has not expired yet, do not include the proxy in the resulting collection.</remarks>
    public TimeSpan? ExpiredState { get; private init; }

    /// <summary>
    ///     Builder for filter construction.
    /// </summary>
    public static FilterBuilder Builder => new();

    /// <summary>
    ///     Filter builder.
    /// </summary>
    public sealed class FilterBuilder
    {
        public FilterBuilder Count(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(Count), $"The Count must be greater than 0");
            
            if (_count != null)
                throw new InvalidOperationException("You cannot call the 'Count' method twice"); 

            _count = value;
            return this;
        }

        public FilterBuilder Include(IEnumerable<Proxy> proxies)
        {
            if (proxies == null)
                throw new ArgumentNullException(nameof(proxies));

            _includedProxies.AddRange(proxies);
            return this;
        }

        public FilterBuilder Except(IEnumerable<Proxy> proxies)
        {
            if (proxies == null)
                throw new ArgumentNullException(nameof(proxies));

            _excludedProxies.AddRange(proxies);
            return this;
        }

        public FilterBuilder StateNotExpired(TimeSpan expiredState)
        {
            if (_expiredState != null)
                throw new InvalidOperationException("You cannot call the 'StateNotExpired' method twice"); 

            _expiredState = expiredState;
            return this;
        }

        public FilterBuilder SortedBy(SortingField field, SortDirection direction)
        {
            if (_sort != null)
                throw new InvalidOperationException("You cannot call the 'SortedBy' method twice"); 
                    
            _sort = new ProxySort(field, direction);
            return this;
        }

        public FilterBuilder Valid(bool isValid)
        {
            if (_isValid != null)
                throw new InvalidOperationException("You cannot call the 'Valid' method twice"); 

            _isValid = isValid;
            return this;
        }

        public FilterBuilder Anonimous(bool isAnonimous)
        {
            if (_isAnonimous != null)
                throw new InvalidOperationException("You cannot call the 'Anonimous' method twice"); 

            _isAnonimous = isAnonimous;
            return this;
        }
        
        public Filter Build() => new()
        {
            Count = _count,
            ExcludedProxies = _excludedProxies,
            IncludedProxies = _includedProxies,
            Sort = _sort,
            ExpiredState = _expiredState,
            IsValid = _isValid,
            IsAnonimous = _isAnonimous
        };

        private int? _count;
        private readonly List<Proxy> _excludedProxies = new();
        private readonly List<Proxy> _includedProxies = new();
        private ProxySort? _sort;
        private bool? _isValid;
        private bool? _isAnonimous;
        private TimeSpan? _expiredState;
    }
}