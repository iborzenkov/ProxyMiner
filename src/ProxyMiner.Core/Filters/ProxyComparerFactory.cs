using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Filters;

/// <summary>
///     Factory for building proxy comparers.
/// </summary>
internal static class ProxyComparerFactory
{
    /// <summary>
    ///     Build proxy comparer.
    /// </summary>
    /// <param name="sort">Specifying sorting.</param>
    /// <returns>Proxy comparer</returns>
    /// <exception cref="ArgumentOutOfRangeException">Occurs if the sorting direction is not implemented.</exception>
    internal static IComparer<StateOfProxy> Make(ProxySort sort)
    {
        return sort.Field switch
        {
            SortingField.LastCheck => new LastCheckProxyComparer(sort.Direction),
            _ => throw new ArgumentOutOfRangeException($"Sorting for '{sort.Field}' is not implemented"),
        };
    }

    private class LastCheckProxyComparer : IComparer<StateOfProxy>
    {
        public LastCheckProxyComparer(SortDirection direction)
        {
            _direction = direction;
        }

        public int Compare(StateOfProxy? x, StateOfProxy? y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null) 
                return -1;
            if (y == null)
                return 1;
            if (x.Equals(y))
                return 0;

            var stateX = x.State;
            var stateY = y.State;

            if (stateX == null && stateY == null)
                return 0;
            if (stateX == null)
                return -1;
            if (stateY == null)
                return 1;
            if (stateX == stateY)
                return 0;

            var finishDateX = stateX.FinishTimeUtc;
            var finishDateY = stateY.FinishTimeUtc;

            if (finishDateX == null && finishDateY == null)
                return 0;
            if (finishDateX == null)
                return -1;
            if (finishDateY == null)
                return 1;

            var compare = finishDateX.Value.CompareTo(finishDateY.Value);
            return _direction == SortDirection.Asceding
                ? compare 
                : -1 * compare;
        }

        private readonly SortDirection _direction;
    }
}