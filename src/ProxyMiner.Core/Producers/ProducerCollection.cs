using System.Collections.Concurrent;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Models.BaseCollections;
using ProxyMiner.Core.Options;

namespace ProxyMiner.Core.Producers;

internal sealed class ProducerCollection : IProducerCollection
{
    public ProducerCollection(Settings settings)
    {
        _settings = settings;
    }

    public void Add(Producer producer) => AddRange(new[] { producer });

    public void AddRange(IEnumerable<Producer> producers)
    {
        var addedItems = new List<Producer>();
        foreach (var producer in producers)
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

        if (addedItems.Any())
        {
            OnCollectionChanged(CollectionChangedEventArgs<Producer>.AddEventArgs(addedItems));
        }

        void SessionStart(Producer source, DateTime startTimeUtc)
            => Mining.Invoke(this, new ProxyMiningEventArgs(source, startTimeUtc));
        void SessionDone(Producer source, DateTime finishTime, ProxyProviderResult sessionResult)
            => Mined.Invoke(this, new ProxyMinedEventArgs(source, finishTime, sessionResult));
    }


    public void Remove(Producer producer) => RemoveRange(new[] { producer });

    public void RemoveRange(IEnumerable<Producer> producers)
    {
        var removedItems = new List<Producer>();
        foreach (var producer in producers)
        {
            if (!_items.TryRemove(producer, out var sourceTask))
                continue;

            producer.EnabledChanged -= ProducerEnabledChanged;

            TryStopTaskOf(producer);
            sourceTask.Dispose();

            removedItems.Add(producer);
        }

        if (removedItems.Any())
        {
            OnCollectionChanged(CollectionChangedEventArgs<Producer>.RemoveEventArgs(removedItems));
        }
    }

    public IEnumerable<Producer> Items => _items.Keys;

    public void Start()
    {
        if (_isActive)
            return;

        _isActive = true;

        foreach (var source in _items.Keys)
        {
            TryStartTaskOf(source);
        }
    }

    public void Stop()
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