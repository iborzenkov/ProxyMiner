using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Checkers;

/// <summary>
///     Arguments of the event about the end of the proxy check.
/// </summary>
public sealed class ProxyCheckedEventArgs : EventArgs
{
    internal ProxyCheckedEventArgs(StateOfProxy stateOfProxy)
    {
        StateOfProxy = stateOfProxy ?? throw new ArgumentNullException(nameof(stateOfProxy));
    }

    /// <summary>
    ///     Proxy state after verification.
    /// </summary>
    public StateOfProxy StateOfProxy { get; }
}