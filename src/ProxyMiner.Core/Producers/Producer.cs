namespace ProxyMiner.Core.Producers;

/// <summary>
///     Proxies producer.
/// </summary>
public sealed class Producer
{
    public Producer(string name, IProxyProvider provider)
    {
        Name = name;
        Provider = provider;
    }

    /// <summary>
    ///     Name of producer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     A sign of the producer's availability.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value)
                return;

            _isEnabled = value;
            EnabledChanged.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///     Provider of proxies.
    /// </summary>
    public IProxyProvider Provider { get; }

    /// <summary>
    ///     Event about producer availability change.
    /// </summary>
    public event EventHandler<EventArgs> EnabledChanged = (_, _) => { };

    private bool _isEnabled;
}
