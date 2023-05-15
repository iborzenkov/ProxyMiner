namespace ProxyMiner.Filters;

public sealed class Filter
{
    private Filter() 
    { }

    public int? Count { get; private init; }
    
    public IEnumerable<Proxy> ExcludedProxies { get; private init; } = Enumerable.Empty<Proxy>();
    public IEnumerable<Proxy> IncludedProxies { get; private init; } = Enumerable.Empty<Proxy>();
    
    public ProxySort? Sort { get; private init; }
    
    public bool? IsValid { get; private init; }
    
    public bool? IsAnonimous { get; private init; }

    public TimeSpan? ExpiredState { get; private init; }

    public static FilterBuilder Builder => new();

    public class FilterBuilder
    {
        public FilterBuilder Count(int value)
        {
            _count = value;
            return this;
        }

        public FilterBuilder Include(IEnumerable<Proxy> proxies)
        {
            _includedProxies = proxies;
            return this;
        }

        public FilterBuilder Except(IEnumerable<Proxy> proxies)
        {
            _excludedProxies = proxies;
            return this;
        }

        public FilterBuilder StateNotExpired(TimeSpan expiredState)
        {
            _expiredState = expiredState;
            return this;
        }

        public FilterBuilder SortedBy(SortingField field, SortDirection direction)
        {
            _sort = new ProxySort(field, direction);
            return this;
        }

        public FilterBuilder Valid(bool isValid)
        {
            _isValid = isValid;
            return this;
        }

        public FilterBuilder Anonimous(bool isAnonimous)
        {
            _isAnonimous = isAnonimous;
            return this;
        }
        
        public Filter Build() 
        { 
            return new Filter 
            {
                Count = _count,
                ExcludedProxies = _excludedProxies ?? Enumerable.Empty<Proxy>(),
                IncludedProxies = _includedProxies ?? Enumerable.Empty<Proxy>(),
                Sort = _sort,
                ExpiredState = _expiredState,
                IsValid = _isValid,
                IsAnonimous = _isAnonimous
            };
        }

        private int? _count;
        private IEnumerable<Proxy>? _excludedProxies;
        private IEnumerable<Proxy>? _includedProxies;
        private ProxySort? _sort;
        private bool? _isValid;
        private bool? _isAnonimous;
        private TimeSpan? _expiredState;
    }
}
