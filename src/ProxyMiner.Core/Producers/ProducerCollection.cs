using System.Collections.Concurrent;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Options;

namespace ProxyMiner.Core.Producers;

internal sealed class ProducerCollection : IProducerCollection
{
    public ProducerCollection(Settings settings)
    {
        _settings = settings;
    }

    public void Add(Producer producer)
    {
        if (_items.ContainsKey(producer))
            return;

        if (_items.TryAdd(producer, new ProducerTask(producer, _settings, SessionStart, SessionDone)))
        {
            producer.EnabledChanged += ProducerEnabledChanged;

            TryStartTaskOf(producer);
            
            OnCollectionChanged();
        }

        void SessionStart(Producer source, DateTime startTime) 
            => Mining.Invoke(this, new ProxyMiningEventArgs(source, startTime));
        void SessionDone(Producer source, DateTime finishTime, IEnumerable<Proxy> proxies) 
            => Mined.Invoke(this, new ProxyMinedEventArgs(source, finishTime, proxies));
    }

    public void Remove(Producer producer)
    {
        if (!_items.TryRemove(producer, out var sourceTask))
            return;

        producer.EnabledChanged -= ProducerEnabledChanged;

        TryStopTaskOf(producer);
        sourceTask.Dispose();

        OnCollectionChanged();
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
    
    public event EventHandler<EventArgs> CollectionChanged = (_, _) => { };

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

    private void OnCollectionChanged() => CollectionChanged.Invoke(this, EventArgs.Empty);

    private readonly Settings _settings;
    private readonly ConcurrentDictionary<Producer, ProducerTask> _items = new();

    private bool _isActive;
}