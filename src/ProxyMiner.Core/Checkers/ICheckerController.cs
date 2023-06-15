using ProxyMiner.Core.Filters;
using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Checkers;

/// <summary>
///     Controller that manage proxy validation.
/// </summary>
public interface ICheckerController : IDisposable
{
    /// <summary>
    ///     Instructs to check the specified proxy collection as much as possible.
    /// </summary>
    /// <param name="proxies">Proxy collection.</param>
    void CheckNow(IEnumerable<Proxy> proxies);

    /// <summary>
    ///     Instructs to exclude the specified proxy collection from the check.
    /// </summary>
    /// <param name="proxies">Proxy collection.</param>
    void StopChecking(IEnumerable<Proxy> proxies);

    /// <summary>
    ///     Event about the beginning of proxy verification.
    /// </summary>
    event EventHandler<ProxyCheckingEventArgs> Checking;

    /// <summary>
    ///     Event about the end of proxy verification.
    /// </summary>
    event EventHandler<ProxyCheckedEventArgs> Checked;
}