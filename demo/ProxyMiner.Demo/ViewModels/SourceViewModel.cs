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
        
        public int? DurationInSec
        {
            get
            {
                return (StartTimeUtc == null || FinishTimeUtc == null) 
                    ? null
                    : (FinishTimeUtc - StartTimeUtc).Value.Seconds;
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

        public int? ValidCount => null;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ProxyMining(object? sender, ProxyMiningEventArgs args)
        {
            if (args.Producer != _producer)
                return;

            StartTimeUtc = args.StartTimeUtc;
            FinishTimeUtc = null;
            TotalCount = null;
        }

        private void ProxyMined(object? sender, ProxyMinedEventArgs args)
        {
            if (args.Producer != _producer)
                return;

            FinishTimeUtc = args.FinishTimeUtc;
            TotalCount = args.Proxies.Count();
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
    }
}