namespace ProxyMiner.Checkers;

public interface IChecker
{
    Task<ProxyStatus> Check(Proxy proxy, CancellationToken token);
}