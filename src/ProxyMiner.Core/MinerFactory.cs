using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Options;

namespace ProxyMiner.Core;

/// <summary>
///     Miner factory.
/// </summary>
public class MinerFactory
{
    /// <summary>
    ///     Creates a IMiner.
    /// </summary>
    /// <param name="checker">Proxy checker.</param>
    /// <param name="settingsProvider">Provider of the settings.</param>
    /// <returns>Miner.</returns>
    public static IMiner GetMiner(IChecker checker, ISettingsProvider settingsProvider) 
        => new Miner(checker, settingsProvider);
}
