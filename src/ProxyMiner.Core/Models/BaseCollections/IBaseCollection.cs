namespace ProxyMiner.Core.Models.BaseCollections;

public interface IBaseCollection<T>
{
    /// <summary>
    ///     Adds one item to the collection.
    /// </summary>
    /// <param name="item">Item being added.</param>
    void Add(T item);

    /// <summary>
    ///     Adds multiple items to the collection.
    /// </summary>
    /// <param name="items">Added items.</param>
    void AddRange(IEnumerable<T> items);

    /// <summary>
    ///     Removes one item from the collection.
    /// </summary>
    /// <param name="item">Item being removed.</param>
    void Remove(T item);

    /// <summary>
    ///     Removes multiple items from the collection.
    /// </summary>
    /// <param name="items">Removing items.</param>
    void RemoveRange(IEnumerable<T> items);

    /// <summary>
    ///     Collection elements.
    /// </summary>
    IEnumerable<T> Items { get; }

    /// <summary>
    ///     Event about changing items in the collection.
    /// </summary>
    event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged;
}
