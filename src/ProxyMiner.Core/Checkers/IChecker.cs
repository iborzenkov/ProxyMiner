using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Checkers;

public interface IChecker
{
    /// <summary>
    ///     Checks the specified proxy for availability.
    /// </summary>
    /// <param name="proxy">The proxy being checked.</param>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>Proxy status.</returns>
    Task<ProxyStatus> Check(Proxy proxy, CancellationToken token);
}