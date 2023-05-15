using ProxyMiner.Filters;

namespace ProxyMiner;

internal static class ProxyComparerFactory
{
    internal static IComparer<Proxy> Make(ProxySort sort, Func<Proxy, ProxyState> stateFinder)
    {
        return sort.Field switch
        {
            SortingField.LastCheck => new LastCheckProxyComparer(sort.Direction, stateFinder),
            _ => throw new ArgumentOutOfRangeException($"Sorting for '{sort.Field}' not implemented")
        };
    }

    private class LastCheckProxyComparer : IComparer<Proxy>
    {
        public LastCheckProxyComparer(SortDirection direction, Func<Proxy, ProxyState> stateFinder)
        {
            _direction = direction;
            _stateFinder = stateFinder;
        }

        public int Compare(Proxy? x, Proxy? y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null) 
                return -1;
            if (y == null)
                return 1;
            if (x.Equals(y))
                return 0;

            var stateX = _stateFinder(x);
            var stateY = _stateFinder(y);

            if (stateX == null && stateY == null)
                return 0;
            if (stateX == null)
                return -1;
            if (stateY == null)
                return 1;
            if (stateX == stateY)
                return 0;

            var finishDateX = stateX.FinishTime;
            var finishDateY = stateY.FinishTime;

            if (finishDateX == null && finishDateY == null)
                return 0;
            if (finishDateX == null)
                return -1;
            if (finishDateY == null)
                return 1;

            return finishDateX.Value.CompareTo(finishDateY.Value);
        }

        private SortDirection _direction;
        private readonly Func<Proxy, ProxyState> _stateFinder;
    }
}