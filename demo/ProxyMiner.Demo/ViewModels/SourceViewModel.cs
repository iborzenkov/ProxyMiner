using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ProxyMiner.Core.Producers;

namespace ProxyMiner.Demo.ViewModels
{
    public sealed class SourceViewModel : INotifyPropertyChanged, IDisposable
    {
        public SourceViewModel(Producer producer, IProducerCollection producers)
        {
            _producer = producer;

            _producers = producers;
            _producers.Mining += ProxyMining;
            _producers.Mined += ProxyMined;
        }

        public void Dispose()
        {
            _producers.Mining -= ProxyMining;
            _producers.Mined -= ProxyMined;
        }

        public string Name => _producer.Name;

        public bool IsEnabled
        {
            get => _producer.IsEnabled;
            set => _producer.IsEnabled = value;
        }

        public DateTime? StartTimeUtc
        {
            get => _startTimeUtc;
            private set
            {
                _startTimeUtc = value;
                OnChanged();
                OnChanged(nameof(DurationInSec));
            }
        }
        public DateTime? FinishTimeUtc
        {
            get => _finishTimeUtc;
            private set
            {
                _finishTimeUtc = value;
                OnChanged();
                OnChanged(nameof(DurationInSec));
            }
        }
        
        public double? DurationInSec
        {
            get
            {
                return (StartTimeUtc == null || FinishTimeUtc == null) 
                    ? null
                    : (FinishTimeUtc - StartTimeUtc).Value.TotalSeconds;
            }
        }
        
        public int? TotalCount 
        {
            get => _totalCount;
            private set
            {
                _totalCount = value;
                OnChanged();
            }
        }

        public ProxyProviderResultCode? ResultCode
        {
            get => _resultCode;
            private set
            {
                _resultCode = value;
                OnChanged();
            }
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                _errorMessage = value;
                OnChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ProxyMining(object? sender, ProxyMiningEventArgs args)
        {
            if (args.Producer != _producer)
                return;

            StartTimeUtc = args.StartTimeUtc;
            FinishTimeUtc = null;
            TotalCount = null;
            ResultCode = null;
            ErrorMessage = null;
        }

        private void ProxyMined(object? sender, ProxyMinedEventArgs args)
        {
            if (args.Producer != _producer)
                return;

            FinishTimeUtc = args.FinishTimeUtc;
            TotalCount = args.MiningResult.Proxies.Count();
            ResultCode = args.MiningResult.Code;
            ErrorMessage = args.MiningResult.Code == ProxyProviderResultCode.Custom
                ? args.MiningResult.CustomMessage
                : args.MiningResult.Code == ProxyProviderResultCode.Error
                    ? args.MiningResult.Exception!.Message
                    : null;
        }

        private void OnChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly Producer _producer;
        private readonly IProducerCollection _producers;
        
        private DateTime? _startTimeUtc;
        private DateTime? _finishTimeUtc;
        private int? _totalCount;
        private ProxyProviderResultCode? _resultCode;
        private string? _errorMessage;
    }
}