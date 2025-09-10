using System.Collections.Concurrent;
using ProxyMiner.Core.Models.BaseCollections;
using ProxyMiner.Core.Options;

namespace ProxyMiner.Core.Producers;

internal sealed class ProducerCollection : IProducerCollection
{
    internal ProducerCollection(Settings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public void Add(Producer producer)
    {
        ArgumentNullException.ThrowIfNull(producer);

        AddRange([producer]);
    }

    public void AddRange(IEnumerable<Producer?> producers)
    {
        ArgumentNullException.ThrowIfNull(producers);

        var addedItems = new List<Producer>();
        foreach (var producer in producers.Where(p => p != null).ToList())
        {
            if (_items.ContainsKey(producer))
                continue;

            if (_items.TryAdd(producer, new ProducerTask(producer, _settings, SessionStart, SessionDone)))
            {
                producer.EnabledChanged += ProducerEnabledChanged;

                TryStartTaskOf(producer);
                
                addedItems.Add(producer);
            }
        }

        if (addedItems.Count != 0)
        {
            OnCollectionChanged(CollectionChangedEventArgs<Producer>.AddEventArgs(addedItems));
        }

        void SessionStart(Producer source, DateTime startTimeUtc)
            => Mining.Invoke(this, new ProxyMiningEventArgs(source, startTimeUtc));
        void SessionDone(Producer source, DateTime finishTime, ProxyProviderResult sessionResult)
            => Mined.Invoke(this, new ProxyMinedEventArgs(source, finishTime, sessionResult));
    }


    public void Remove(Producer producer)
    {
        ArgumentNullException.ThrowIfNull(producer);

        RemoveRange([producer]);
    }

    public void RemoveRange(IEnumerable<Producer?> producers)
    {
        ArgumentNullException.ThrowIfNull(producers);

        var removedItems = new List<Producer>();
        foreach (var producer in producers.Where(p => p != null))
        {
            if (!_items.TryRemove(producer, out var sourceTask))
                continue;

            producer.EnabledChanged -= ProducerEnabledChanged;

            TryStopTaskOf(producer);
            sourceTask.Dispose();

            removedItems.Add(producer);
        }

        if (removedItems.Count != 0)
        {
            OnCollectionChanged(CollectionChangedEventArgs<Producer>.RemoveEventArgs(removedItems));
        }
    }

    public IEnumerable<Producer> Items => _items.Keys;

    internal void Start()
    {
        if (_isActive)
            return;

        _isActive = true;

        foreach (var source in _items.Keys)
        {
            TryStartTaskOf(source);
        }
    }

    internal void Stop()
    {
        if (!_isActive)
            return;

        _isActive = false;

        foreach (var source in Items)
        {
            TryStopTaskOf(source);
        }
    }

    public event EventHandler<ProxyMiningEventArgs> Mining = (_, _) => { };
    public event EventHandler<ProxyMinedEventArgs> Mined = (_, _) => { };
    
    public event EventHandler<CollectionChangedEventArgs<Producer>> CollectionChanged = (_, _) => { };

    private void TryStartTaskOf(Producer producer)
    {
        if (_isActive && producer.IsEnabled && _items.TryGetValue(producer, out var task))
        {
            task.Start();
        }
    }

    private void TryStopTaskOf(Producer producer)
    {
        if (_items.TryGetValue(producer, out var task))
        {
            task.Stop();
        }
    }

    private void ProducerEnabledChanged(object? sender, EventArgs e)
    {
        if (!_isActive || sender is not Producer producer)
            return;

        if (producer.IsEnabled)
        {
            TryStartTaskOf(producer);
        }
        else
        {
            TryStopTaskOf(producer);
        }
    }

    private void OnCollectionChanged(CollectionChangedEventArgs<Producer> args) => CollectionChanged.Invoke(this, args);

    private readonly Settings _settings;
    private readonly ConcurrentDictionary<Producer, ProducerTask> _items = new();

    private bool _isActive;
}