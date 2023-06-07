using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models.ProxyCollections;
using ProxyMiner.Core.Options;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Core;

internal sealed class Miner : IMiner
{
    public Miner(IChecker checker, ISettingsProvider settingsProvider)
    {
        var settings = settingsProvider.Settings;

        _producers = new ProducerCollection(settings);
        _producers.Mined += ProxiesMined;

        _proxies = new ProxyCollection();

        _proxyChecker = new ProxyChecker(checker, settings);
        
        _proxyCheckerController = new ProxyCheckerController(_proxies, _proxyChecker, settings);
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
}