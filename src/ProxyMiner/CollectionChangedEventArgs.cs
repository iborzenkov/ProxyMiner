namespace ProxyMiner;

public sealed class CollectionChangedEventArgs<T>
{
    private CollectionChangedEventArgs(CollectionChangedAction action, 
        ICollection<T>? newItems, ICollection<T>? oldItems)
    {
        Action = action;
        NewItems = newItems;
        OldItems = oldItems;
    }

    internal static CollectionChangedEventArgs<T> RemoveEventArgs(ICollection<T> items) 
        => new (CollectionChangedAction.Remove, null, items);

    internal static CollectionChangedEventArgs<T> AddEventArgs(ICollection<T> items)
        => new (CollectionChangedAction.Add, items, null);

    public CollectionChangedAction Action { get; }
    public ICollection<T>? OldItems { get; }
    public ICollection<T>? NewItems { get; }
}

public enum CollectionChangedAction
{
    Add,
    Remove
}