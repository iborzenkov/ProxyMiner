using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models.ProxyCollections;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Core;

public interface IMiner
{
    void Start();
    void Stop();

    IProducerCollection Producers { get; }
    IProxyCollection Proxies { get; }
    ICheckerController Checker { get; }
}
