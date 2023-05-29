using ProxyMiner.Core.Models.BaseCollections;

namespace ProxyMiner.Core.Producers;

/// <summary>
///     Producer collection.
/// </summary>
public interface IProducerCollection : IBaseCollection<Producer>
{
    /// <summary>
    ///     Event about the beginning of the proxy search in the source.
    /// </summary>
    event EventHandler<ProxyMiningEventArgs> Mining;

    /// <summary>
    ///     Event about the end of the proxy search in the source.
    /// </summary>
    event EventHandler<ProxyMinedEventArgs> Mined;
}