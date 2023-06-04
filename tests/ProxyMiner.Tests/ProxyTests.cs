using ProxyMiner.Core.Models;

namespace ProxyMiner.Tests;

[TestClass]
public class ProxyTests
{
    [TestMethod]
    [DataRow(ProxyType.Http, "127.0.0.1", 80, true, null)]
    [DataRow(ProxyType.Http, "127.0.0.1", 0, true, null)]
    [DataRow(ProxyType.Http, "127.0.0.1", 65535, true, null)]
    [DataRow(ProxyType.Http, "127.0.0.1", -1, false, MakeProxyError.PortOutOfRange)]
    [DataRow(ProxyType.Http, "127.0.0.1", 65536, false, MakeProxyError.PortOutOfRange)]
    [DataRow(ProxyType.Http, null, 80, false, MakeProxyError.HostIsNull)]
    [DataRow(ProxyType.Http, "", 80, false, MakeProxyError.HostIsNull)]
    [DataRow(ProxyType.Http, "127.0.0.1/32", 80, false, MakeProxyError.HostIsNotCorrect)]
    [DataRow(ProxyType.Http, "localhost", 80, false, MakeProxyError.HostIsNotCorrect)]
    [DataRow(ProxyType.Http, "localhost1", 80, false, MakeProxyError.HostIsNotCorrect)]
    [DataRow(ProxyType.Http, "127.0.0", 80, false, MakeProxyError.HostIsNotCorrect)]
    [DataRow(ProxyType.Http, "127.0.0.0.1", 80, false, MakeProxyError.HostIsNotCorrect)]
    [DataRow(ProxyType.Http, "256.0.0.1", 80, false, MakeProxyError.HostIsNotCorrect)]
    public void ProxyFactory_MakeProxy(ProxyType type, string host, int port,
        bool isSuccess, MakeProxyError? proxyError)
    {
        var (proxy, actualProxyError) = Proxy.Factory.TryMakeProxy(
            type, host, port);
        
        Assert.AreEqual(isSuccess, proxy != null);
        Assert.AreEqual(proxyError, actualProxyError);
    }
}