﻿using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Options;

namespace ProxyMiner.Core;

/// <summary>
///     Miner factory.
/// </summary>
public static class MinerFactory
{
    /// <summary>
    ///     Creates an IMiner.
    /// </summary>
    /// <param name="checker">Proxy checker.</param>
    /// <param name="settingsProvider">Provider of the settings.</param>
    /// <returns>Miner.</returns>
    public static IMiner GetMiner(IChecker checker, ISettingsProvider settingsProvider)
    {
        if (checker == null)
            throw new ArgumentNullException(nameof(checker));

        if (settingsProvider == null)
            throw new ArgumentNullException(nameof(settingsProvider));

        return new Miner(checker, settingsProvider);
    }
}