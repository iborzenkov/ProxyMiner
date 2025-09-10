namespace ProxyMiner.Core.Producers;

/// <summary>
///     Proxies producer.
/// </summary>
public sealed class Producer
{
    public Producer(string name, IProxyProvider provider)
    {
        Name = ValidateName(name);
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <summary>
    ///     Name of producer.
    /// </summary>
    public string Name
    {
        get => _name!;
        set
        {
            if (_name != null && _name.Equals(value, StringComparison.CurrentCulture))
                return;
            
            _name = ValidateName(value);
        }
    }

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

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))    
            throw new ArgumentNullException(nameof(name));
        
        return name;
    }
    
    private bool _isEnabled;
    private string? _name;
}