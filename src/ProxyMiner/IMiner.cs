using ProxyMiner.Producers;

namespace ProxyMiner;

public interface IMiner
{
    void Start();
    void Stop();

    IProducerCollection Producers { get; }
    IProxyCollection Proxies { get; }
    ICheckerController Checker { get; }
}
