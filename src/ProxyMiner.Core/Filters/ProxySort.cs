namespace ProxyMiner.Core.Filters;

/// <summary>
///     Specifying sorting of proxies collection.
/// </summary>
public sealed record ProxySort(SortingField Field, SortDirection Direction)
{
    /// <summary>
    ///     Sorting direction.
    /// </summary>
    public SortDirection Direction { get; } = Direction;

    /// <summary>
    ///     Sortable proxy field.
    /// </summary>
    public SortingField Field { get; } = Field;
}