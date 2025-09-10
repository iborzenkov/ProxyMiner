using Moq;
using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models;

namespace ProxyMiner.Tests.Moqs;

internal class CheckerBuilder
{
    public CheckerBuilder Status(ProxyStatus status)
    {
        _status = status ?? throw new ArgumentNullException(nameof(status));
        return this;
    }

    public CheckerBuilder TimeForWork(TimeSpan delay)
    {
        _delay = delay;
        return this;
    }

    public IChecker Build()
    {
        var providerMock = new Mock<IChecker>();
        providerMock.Setup(p => p.Check(It.IsAny<Proxy>(), It.IsAny<CancellationToken>()))
            .Returns(new InvocationFunc(i => Task.Run(() =>
            {
                var token = (CancellationToken)i.Arguments[1];
                if (_delay != null)
                {
                    token.WaitHandle.WaitOne((int)_delay.Value.TotalMilliseconds);
                }

                token.ThrowIfCancellationRequested();
                return _status ?? ProxyStatus.NotAnonimous;
            })));

        return providerMock.Object;
    }

    private ProxyStatus? _status;
    private TimeSpan? _delay;
}
