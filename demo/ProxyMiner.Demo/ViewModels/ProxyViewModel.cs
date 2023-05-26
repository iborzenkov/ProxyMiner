using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProxyMiner.Core;
using ProxyMiner.Core.Checkers;
using ProxyMiner.Core.Models;

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
        
        public DateTime? StartCheckUtc
        {
            get => _startCheckUtc;
            private set
            {
                _startCheckUtc = value;
                
                OnChanged();
                OnChanged(nameof(DurationInSec));
            }
        }

        public DateTime? FinishCheckUtc
        {
            get => _finishCheckUtc;
            private set
            {
                _finishCheckUtc = value;
                
                OnChanged();
                OnChanged(nameof(DurationInSec));
            }
        }

        public int? DurationInSec
        {
            get
            {
                return (StartCheckUtc == null || FinishCheckUtc == null) 
                    ? null
                    : (FinishCheckUtc - StartCheckUtc).Value.Seconds;
            }
        }

        public Proxy Proxy { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ProxyChecking(object? sender, ProxyCheckingEventArgs args)
        {
            if (args.Proxy.Equals(Proxy))
            {
                StartCheckUtc = args.StartTimeUtc;
                FinishCheckUtc = null;
                SetStatus(null);
            }
        }

        private void ProxyChecked(object? sender, ProxyCheckedEventArgs args)
        {
            if (args.Proxy.Equals(Proxy))
            {
                SetStatus(args.State.Status);
                StartCheckUtc = args.State.StartTimeUtc;
                FinishCheckUtc = args.State.FinishTimeUtc;
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
        private DateTime? _startCheckUtc;
        private DateTime? _finishCheckUtc;
    }
}