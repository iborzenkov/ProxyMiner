using Moq;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Tests.Moqs
{
    internal class ProxyProviderBuilder
    {
        public ProxyProviderBuilder ReturnOk() => ReturnOk(Enumerable.Empty<Proxy>());
        
        public ProxyProviderBuilder ReturnOk(IEnumerable<Proxy> proxies)
        {
            if (_isTimeout != null)
                throw new InvalidOperationException("You cannot call the 'ReturnOk' after 'ReturnTimeout'");

            _proxies = proxies ?? throw new ArgumentNullException(nameof(proxies));
            return this;
        }

        public ProxyProviderBuilder ReturnTimeout()
        {
            if (_proxies != null)
                throw new InvalidOperationException("You cannot call the 'ReturnTimeout' after 'ReturnOk'");

            _isTimeout = true;
            return this;
        }

        public ProxyProviderBuilder TimeForWork(TimeSpan delay)
        {
            _delay = delay;
            return this;
        }

        public IProxyProvider Build()
        {
            var providerMock = new Mock<IProxyProvider>();
            providerMock.Setup(p => p.GetProxies(It.IsAny<CancellationToken>()))
                .Returns(new InvocationFunc(i => Task.Run(() =>
                {
                    var token = (CancellationToken)i.Arguments[0];
                    if (_delay != null)
                    {
                        token.WaitHandle.WaitOne((int)_delay.Value.TotalMilliseconds);
                    }

                    token.ThrowIfCancellationRequested();
                    if (_proxies != null)
                        return ProxyProviderResult.Ok(_proxies);
                    if (_isTimeout != null && _isTimeout.Value)
                        return ProxyProviderResult.Timeout;
                    
                    throw new InvalidOperationException("Result not defined");
                })));

            return providerMock.Object;
        }

        private IEnumerable<Proxy>? _proxies;
        private TimeSpan? _delay;
        private bool? _isTimeout;
    }
}
