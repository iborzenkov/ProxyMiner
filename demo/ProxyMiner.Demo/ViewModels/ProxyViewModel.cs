using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProxyMiner.Demo.ViewModels
{
    public sealed class ProxyViewModel : INotifyPropertyChanged, IDisposable
    {
        public ProxyViewModel(Proxy proxy, ICheckerController checker)
        {
            Proxy = proxy;

            _checker = checker;
            _checker.Checking += ProxyChecking;
            _checker.Checked += ProxyChecked;
        }

        public void Dispose()
        {
            _checker.Checking -= ProxyChecking;
            _checker.Checked -= ProxyChecked;
        }

        public string Name => $"{Proxy.Host}:{Proxy.Port}";

        public string Type => Proxy.Type.ToString();
        public string Host => Proxy.Host;
        public int Port => Proxy.Port;
        public string? Username => Proxy.Username;
        public string? Password => Proxy.Password;
        public string? Status
        {
            get => _status;
            private set
            {
                _status = value;
                OnChanged();
            }
        }
        public bool? IsValid { get; private set; }
        public bool? IsAnonimous { get; private set; }
        
        public DateTime? StartCheck
        {
            get => _startCheck;
            private set
            {
                _startCheck = value;
                
                OnChanged();
                OnChanged(nameof(DurationInSec));
            }
        }

        public DateTime? FinishCheck
        {
            get => _finishCheck;
            private set
            {
                _finishCheck = value;
                
                OnChanged();
                OnChanged(nameof(DurationInSec));
            }
        }

        public int? DurationInSec
        {
            get
            {
                return (StartCheck == null || FinishCheck == null) 
                    ? null
                    : (FinishCheck - StartCheck).Value.Seconds;
            }
        }

        public Proxy Proxy { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ProxyChecking(object? sender, ProxyCheckingEventArgs args)
        {
            if (args.Proxy.Equals(Proxy))
            {
                StartCheck = args.StartTime;
                FinishCheck = null;
                SetStatus(null);
            }
        }

        private void ProxyChecked(object? sender, ProxyCheckedEventArgs args)
        {
            if (args.Proxy.Equals(Proxy))
            {
                SetStatus(args.State.Status);
                StartCheck = args.State.StartTime;
                FinishCheck = args.State.FinishTime;
            }
        }

        private void SetStatus(ProxyStatus? status)
        {
            Status = status?.ToString();
            IsValid = status?.IsValid ?? false;
            IsAnonimous = status?.IsAnonimous ?? false;
        }

        private void OnChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly ICheckerController _checker;
 
        private string? _status;
        private DateTime? _startCheck;
        private DateTime? _finishCheck;
    }
}