using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models.ProxyCollections;
using ProxyMiner.Core.Options;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Core;

public sealed class Miner : IMiner, IDisposable
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

    public IProducerCollection Producers => _producers;
    public IProxyCollection Proxies => _proxies;
    public ICheckerController Checker => _proxyCheckerController;

    public void Start()
    {
        _producers.Start();
        _proxyChecker.Start();
    }

    public void Stop()
    {
        _producers.Stop();
        _proxyChecker.Stop();
    }

    public void Dispose()
    {
        _proxyChecker.Dispose();
        _proxyCheckerController.Dispose();
        
        _producers.Mined -= ProxiesMined;
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
    private readonly ProxyCheckerController _proxyCheckerController;
}