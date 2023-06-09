﻿namespace ProxyMiner.Core.Models.BaseCollections;

/// <summary>
///     Event arguments about actions with collection elements.
/// </summary>
public sealed class CollectionChangedEventArgs<T> : EventArgs
{
    private CollectionChangedEventArgs(CollectionChangeAction action, 
        ICollection<T>? newItems, ICollection<T>? oldItems)
    {
        Action = action;
        NewItems = newItems;
        OldItems = oldItems;
    }

    internal static CollectionChangedEventArgs<T> RemoveEventArgs(ICollection<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        return new CollectionChangedEventArgs<T>(CollectionChangeAction.Remove, null, items);
    }

    internal static CollectionChangedEventArgs<T> AddEventArgs(ICollection<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));
        
        return new CollectionChangedEventArgs<T>(CollectionChangeAction.Add, items, null);
    }

    /// <summary>
    ///     Collection change action.
    /// </summary>
    public CollectionChangeAction Action { get; }

    /// <summary>
    ///     List of old items in the collection.
    /// </summary>
    /// <remarks>Used when deleting items from a collection.</remarks>
    public ICollection<T>? OldItems { get; }

    /// <summary>
    ///     List of new items in the collection.
    /// </summary>
    /// <remarks>Used when adding items to a collection.</remarks>
    public ICollection<T>? NewItems { get; }
}