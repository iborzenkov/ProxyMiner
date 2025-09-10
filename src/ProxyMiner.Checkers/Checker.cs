using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models;

namespace ProxyMiner.Checkers;

/// <summary>
///     Checks the selected proxy server.
///     Verification criteria:
///         1. Retrieves the content of an accessible SSL-enabled site.
///         2. They will check it on the whatismyip.cz website is the current IP address. 
///             If it matches the proxy address, then the proxy is anonymous.
/// </summary>
public sealed class Checker : IChecker
{
    /// <summary>
    ///     Checks the proxy server for validity and anonymity.
    /// </summary>
    /// <param name="proxy">The proxy server to check.</param>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>The proxy status indicating whether the proxy is working and if it's anonymous.</returns>
    public Task<ProxyStatus> Check(Proxy proxy, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            token.ThrowIfCancellationRequested();
            var webProxy = new WebProxy
            {
                Address = proxy.Uri,
                Credentials = GetCredentials(proxy)
            };
            
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = webProxy,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13
            };
            using var client = new HttpClient(httpClientHandler, disposeHandler: true);
            
            var statusCode = await IsHttpsConnectionPermitted(client, token);
            if (statusCode != HttpStatusCode.OK)
                return ProxyStatus.ErrorStatus(statusCode);

            var isAnonimousProxy = await IsAnonimousProxy(client, proxy, token);
            return isAnonimousProxy
                ? ProxyStatus.Anonimous
                : ProxyStatus.NotAnonimous;
        },
        token);
    }

    private static async Task<HttpStatusCode> IsHttpsConnectionPermitted(
        HttpClient client, CancellationToken token)
    {
        try
        {
            var response = await client.GetAsync(
                AlwaysAvailableResourceUrl, HttpCompletionOption.ResponseHeadersRead, token);
            return response.StatusCode;
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (SocketException ex)
        {
            return ex.SocketErrorCode == SocketError.TimedOut
                ? HttpStatusCode.RequestTimeout
                : HttpStatusCode.BadRequest;
        }
        catch (HttpRequestException ex)
        {
            if (ex.InnerException is SocketException { SocketErrorCode: SocketError.TimedOut })
                return HttpStatusCode.RequestTimeout;

            return ex.StatusCode ?? HttpStatusCode.BadRequest;
        }
        catch (OperationCanceledException)
        {
            return HttpStatusCode.RequestTimeout;
        }
        catch (Exception)
        {
            return HttpStatusCode.BadRequest;
        }
    }

    private static async Task<bool> IsAnonimousProxy(
        HttpClient client, Proxy proxy, CancellationToken token)
    {
        try
        {
            var response = await client.GetAsync(
                IpCheckerUrl, HttpCompletionOption.ResponseContentRead, token);
            if (!response.IsSuccessStatusCode)
                return false;

            var doc = new HtmlDocument();
            doc.LoadHtml(await response.Content.ReadAsStringAsync(token));
            var ipValue = doc.DocumentNode.QuerySelector(IpCheckerTag)?.InnerText;
            var isAnonimousProxy = proxy.Host.Equals(ipValue, StringComparison.OrdinalIgnoreCase);
            return isAnonimousProxy;
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static NetworkCredential? GetCredentials(Proxy proxy)
    {
        return proxy.AuthorizationData == null
            ? null
            : new NetworkCredential(
                proxy.AuthorizationData.Username,
                proxy.AuthorizationData.Password);
    }

    private const string AlwaysAvailableResourceUrl = "https://ng-ukom.ru/";

    private const string IpCheckerUrl = "http://www.whatismyip.cz/";
    private const string IpCheckerTag = "div.ip";
}