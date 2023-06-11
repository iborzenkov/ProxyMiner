using ProxyMiner.Core.Models;

namespace ProxyMiner.Core.Producers;

/// <summary>
///     Result of getting the proxy list.
/// </summary>
public sealed class ProxyProviderResult
{
    private ProxyProviderResult() { }

    /// <summary>
    ///     Proxies received successfully.
    /// </summary>
    /// <param name="proxies">Proxy collection.</param>
    public static ProxyProviderResult Ok(IEnumerable<Proxy> proxies)
    {
        return new ProxyProviderResult
        {
            Code = ProxyProviderResultCode.Ok, 
            Proxies = proxies ?? Enumerable.Empty<Proxy>(),
        };
    }

    /// <summary>
    ///     Receiving the proxy was interrupted by user.
    /// </summary>
    public static ProxyProviderResult Cancelled => new() { Code = ProxyProviderResultCode.Cancelled };

    /// <summary>
    ///     Receiving the proxy was interrupted by timeout.
    /// </summary>
    public static ProxyProviderResult Timeout => new() { Code = ProxyProviderResultCode.Timeout };

    /// <summary>
    ///     Uncertain result. This is the state before the procedure for getting the proxy list.
    /// </summary>
    public static ProxyProviderResult Unknown => new();

    /// <summary>
    ///     An error occurred while retrieving the proxy list.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    public static ProxyProviderResult Error(Exception exception)
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        return new ProxyProviderResult
        {
            Code = ProxyProviderResultCode.Error,
            Exception = exception
        };
    }

    /// <summary>
    ///     Custom error occurred while retrieving the proxy list.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public static ProxyProviderResult Custom(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(nameof(message));
        
        return new ProxyProviderResult
        {
            Code = ProxyProviderResultCode.Custom, 
            CustomMessage = message
        };
    }

    /// <summary>
    ///     Code results.
    /// </summary>
    public ProxyProviderResultCode Code { get; private init; } = ProxyProviderResultCode.Unknown;
    
    /// <summary>
    ///     Proxy collection.
    /// </summary>
    public IEnumerable<Proxy> Proxies { get; private init; } = Enumerable.Empty<Proxy>();

    /// <summary>
    ///     Error, if any, when receiving the result.
    /// </summary>
    public Exception? Exception { get; private init; }

    /// <summary>
    ///     Custom error message.
    /// </summary>
    public string? CustomMessage { get; private init; }
}