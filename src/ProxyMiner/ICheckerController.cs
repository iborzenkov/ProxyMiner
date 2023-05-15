namespace ProxyMiner;

public interface ICheckerController
{
    void CheckNow(IEnumerable<Proxy> proxies);
    void StopChecking(IEnumerable<Proxy> proxies);

    event EventHandler<ProxyCheckingEventArgs> Checking;
    event EventHandler<ProxyCheckedEventArgs> Checked;
}
