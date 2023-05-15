namespace ProxyMiner
{
    internal interface ICheckObserver
    {
        void EnabledChanged(bool enabled);
        void Checking(ProxyCheckingEventArgs args);
        void Checked(ProxyCheckedEventArgs args);
    }
}