using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Filters;
using ProxyMiner.Core.Models.ProxyCollections;
using ProxyMiner.Core.Options;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Core;

internal sealed class Miner : IMiner
{
    internal Miner(IChecker checker, ISettingsProvider settingsProvider)
    {
        ArgumentNullException.ThrowIfNull(checker);
        ArgumentNullException.ThrowIfNull(settingsProvider);

        var settings = settingsProvider.Settings;

        _producers = new ProducerCollection(settings);
        _producers.Mined += ProxiesMined;

        _proxies = new ProxyCollection();

        _proxyChecker = new ProxyChecker(checker, settings);

        _proxyFilter = new ProxyFilter(_proxies, _proxyChecker);

        _proxyCheckerController = new ProxyCheckerController(
            _proxies, _proxyChecker, _proxyFilter, settings);
    }
    
    void IDisposable.Dispose()
    {
        _proxyChecker.Dispose();
        _proxyCheckerController.Dispose();
        
        _producers.Mined -= ProxiesMined;
    }

    public IProducerCollection Producers => _producers;
    public IProxyCollection Proxies => _proxies;
    public ICheckerController Checker => _proxyCheckerController;
    public IProxyFilter ProxyFilter => _proxyFilter;

    void IMiner.Start()
    {
        _producers.Start();
        _proxyChecker.Start();
    }

    void IMiner.Stop()
    {
        _producers.Stop();
        _proxyChecker.Stop();
    }

    private void ProxiesMined(object? sender, ProxyMinedEventArgs e)
    {
        if (e.MiningResult.Code == ProxyProviderResultCode.Ok)
        {
            _proxies.AddRange(e.MiningResult.Proxies);
        }
    }

    private readonly ProducerCollection _producers;
    private readonly ProxyCollection _proxies;
    private readonly ProxyChecker _proxyChecker;
    private readonly ICheckerController _proxyCheckerController;
    private readonly IProxyFilter _proxyFilter;
}