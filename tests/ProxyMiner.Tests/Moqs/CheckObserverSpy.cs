using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models;

namespace ProxyMiner.Tests.Moqs
{
    internal sealed class CheckObserverSpy : ICheckObserver, IDisposable
    {
        public CheckObserverSpy(List<Proxy>? etalonCheckingProxies, 
            List<Proxy>? etalonCheckedProxies, List<Proxy>? etalonPossibleCheckingProxies)
        {
            _etalonCheckingProxies = etalonCheckingProxies;
            _etalonCheckedProxies = etalonCheckedProxies;
            _etalonPossibleCheckingProxies = etalonPossibleCheckingProxies;
        }

        public void Dispose() => Check();

        public void EnabledChanged(bool enabled)
        {
            //
        }

        public void Checking(ProxyCheckingEventArgs args)
        {
            _actualCheckingProxies.Add(args.Proxy);
        }

        public void Checked(ProxyCheckedEventArgs args)
        {
            _actualCheckedProxies.Add(args.Proxy);
        }

        private void Check()
        {
            if (_etalonCheckingProxies != null)
            {
                CollectionAssert.AreEquivalent(_etalonCheckingProxies, _actualCheckingProxies);
            }

            if (_etalonCheckedProxies != null)
            {
                CollectionAssert.AreEquivalent(_etalonCheckedProxies, _actualCheckedProxies);
            }

            if (_etalonPossibleCheckingProxies != null)
            {
                Assert.IsTrue(_actualCheckingProxies.Count > 0);
                CollectionAssert.IsSubsetOf(_actualCheckingProxies, _etalonPossibleCheckingProxies);
            }
        }

        private readonly List<Proxy>? _etalonCheckingProxies;
        private readonly List<Proxy>? _etalonCheckedProxies;
        private readonly List<Proxy>? _etalonPossibleCheckingProxies;
        private readonly List<Proxy> _actualCheckingProxies = new();
        private readonly List<Proxy> _actualCheckedProxies = new();
    }
}