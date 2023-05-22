using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Checkers;

public interface IChecker
{
    Task<ProxyStatus> Check(Proxy proxy, CancellationToken token);
}