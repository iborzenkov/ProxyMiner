namespace ProxyMiner.Core.Filters;

/// <summary>
///     Specifying sorting of proxies collection.
/// </summary>
public sealed record ProxySort
{
    public ProxySort(SortingField field, SortDirection direction)
    {
        Field = field;
        Direction = direction;
    }

    /// <summary>
    ///     Sorting direction.
    /// </summary>
    public SortDirection Direction { get; }

    /// <summary>
    ///     Sortable proxy field.
    /// </summary>
    public SortingField Field { get; }
}
