namespace ProxyMiner.Core.Producers;

public sealed class Producer
{
    public Producer(string name, IProxyProvider provider)
    {
        Name = name;
        Provider = provider;
    }

    public string Name { get; set; }

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

    public IProxyProvider Provider { get; }

    public event EventHandler<EventArgs> EnabledChanged = (_, _) => { };

    private bool _isEnabled;
}
