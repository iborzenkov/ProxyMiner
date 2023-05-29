namespace ProxyMiner.Core.Producers;

/// <summary>
///     Arguments of the event about the beginning of the proxy search in the source.
/// </summary>
public sealed class ProxyMiningEventArgs : EventArgs
{
    public ProxyMiningEventArgs(Producer producer, DateTime startTimeUtc)
    {
        Producer = producer;
        StartTimeUtc = startTimeUtc;
    }

    /// <summary>
    ///     The producer where the search was started.
    /// </summary>
    public Producer Producer { get; }

    /// <summary>
    ///     Search start time.
    /// </summary>
    public DateTime StartTimeUtc { get; }
}