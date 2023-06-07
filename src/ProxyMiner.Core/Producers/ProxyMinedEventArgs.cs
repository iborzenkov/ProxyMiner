using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Producers;

/// <summary>
///     Arguments of the event about the end of the proxy search in the source.
/// </summary>
public sealed class ProxyMinedEventArgs : EventArgs
{
    internal ProxyMinedEventArgs(Producer producer, DateTime finishTimeUtc, ProxyProviderResult miningResult)
    {
        Producer = producer ?? throw new ArgumentNullException(nameof(producer));
        FinishTimeUtc = finishTimeUtc;
        MiningResult = miningResult ?? throw new ArgumentNullException(nameof(miningResult));
    }

    /// <summary>
    ///     The producer where the proxy search ended.
    /// </summary>
    public Producer Producer { get; }

    /// <summary>
    ///     The end time of the search.
    /// </summary>
    public DateTime FinishTimeUtc { get; }

    /// <summary>
    ///     Result of the mining.
    /// </summary>
    public ProxyProviderResult MiningResult { get; }
}