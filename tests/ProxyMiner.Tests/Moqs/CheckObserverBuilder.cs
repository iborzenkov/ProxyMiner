using ProxyMiner.Core.Models;

namespace ProxyMiner.Tests.Moqs;

internal class CheckObserverBuilder
{
    public CheckObserverBuilder Checking(IEnumerable<Proxy> proxies)
    {
        _etalonCheckingProxies ??= new List<Proxy>();
        _etalonCheckingProxies.AddRange(proxies);
        return this;
    }

    public CheckObserverBuilder Checked(IEnumerable<Proxy> proxies)
    {
        _etalonCheckedProxies ??= new List<Proxy>();
        _etalonCheckedProxies.AddRange(proxies);
        return this;
    }

    public CheckObserverBuilder CheckingFromThese(IEnumerable<Proxy> proxies)
    {
        _etalonPossibleCheckingProxies ??= new List<Proxy>();
        _etalonPossibleCheckingProxies.AddRange(proxies);
        return this;
    }

    public CheckObserverSpy Build()
    {
        return new CheckObserverSpy(
            _etalonCheckingProxies, _etalonCheckedProxies,
            _etalonPossibleCheckingProxies);
    }

    private List<Proxy>? _etalonCheckingProxies;
    private List<Proxy>? _etalonCheckedProxies;
    private List<Proxy>? _etalonPossibleCheckingProxies;
}
