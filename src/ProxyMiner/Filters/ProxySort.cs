namespace ProxyMiner.Filters;

public sealed class ProxySort
{
    public ProxySort(SortingField field, SortDirection direction)
    {
        Field = field;
        Direction = direction;
    }

    public SortDirection Direction { get; }
    public SortingField Field { get; }
}
