using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Input;
using ProxyMiner.Checkers;
using ProxyMiner.Filters;
using ProxyMiner.Options;
using ProxyMiner.Producers;
using ProxyMiner.Producers.ProxyProviders;
using Timer = System.Timers.Timer;

namespace ProxyMiner.Demo.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            var checker = new Checker();
            var settingsProvider = new JsonSettingsProvider("options.json");
            
            _miner = new Miner(checker, settingsProvider);

            _miner.Producers.CollectionChanged += SourceCollectionChanged;
            _miner.Producers.Add(new Producer("Free-Proxy-List", new FreeProxyListProvider()));
            _miner.Producers.Add(new Producer("proxies.txt", new CsvFileProvider("proxies.txt", CsvFileSettings.Default)));
            _miner.Producers.Add(new Producer("brief.txt", new CsvFileProvider("brief.txt", CsvFileSettings.Default)));
            _miner.Producers.Add(new Producer("all.csv", new CsvFileProvider("all.csv", CsvFileSettings.Default)));
            _miner.Producers.Add(new Producer("valid.csv", new CsvFileProvider("valid.csv", CsvFileSettings.Default)));
            _miner.Producers.Add(new Producer("anonimous.csv", new CsvFileProvider("anonimous.csv", CsvFileSettings.Default)));
            _miner.Producers.Add(new Producer("GeoNode", new GeoNodeProvider()));
            _miner.Producers.Add(new Producer("Dummy", new DummyProvider()));

            _miner.Proxies.CollectionChanged += ProxyCollectionChanged;

            StartCommand = new RelayCommand(
                () =>
                {
                    IsActive = true;
                    _miner.Start();
                },
                () => !IsActive);

            StopCommand = new RelayCommand(
                () =>
                {
                    IsActive = false;
                    _miner.Stop();
                },
                () => IsActive);

            AddSourceCommand = new RelayCommand(
                () => _miner.Producers.Add(new Producer("brief.txt", new CsvFileProvider("brief.txt", CsvFileSettings.Default))));
            RemoveSourceCommand = new RelayCommand(
                () => _miner.Producers.Remove(_miner.Producers.Items.Last()),
                () => _miner.Producers.Items.Any());
         
            SaveAllProxies = new RelayCommand(
                () => SaveProxies("all.csv", _miner.Proxies.Items), 
                () => _miner.Proxies.Items.Any());
            SaveValidProxies = new RelayCommand(
                () => SaveProxies("valid.csv", _miner.Proxies.GetProxies(Filter.Builder.Valid(true).Build())), 
                () => _miner.Proxies.Items.Any());
            SaveAnonimousProxies = new RelayCommand(
                () => SaveProxies("anonimous.csv", _miner.Proxies.GetProxies(Filter.Builder.Anonimous(true).Build())), 
                () => _miner.Proxies.Items.Any());
            
            CheckSelectedProxies = new RelayCommand(
                () => _miner.Checker.CheckNow(new []{ SelectedProxy!.Proxy } ),
                () => SelectedProxy != null);

            _refreshStatusTimer = new Timer(_refreshPeriod.TotalMilliseconds) { Enabled = true };
            _refreshStatusTimer.Elapsed += RefreshStatus;
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnChanged();
            }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public ICommand AddSourceCommand { get; }
        public ICommand RemoveSourceCommand { get; }
        
        public ICommand SaveAllProxies { get; }
        public ICommand SaveValidProxies { get; }
        public ICommand SaveAnonimousProxies { get; }
        
        public ICommand CheckSelectedProxies { get; }
        
        public string? ProxyStatus { get; private set; }

        public ObservableCollection<SourceViewModel> Sources { get; private set; } = new();
        public ObservableCollection<ProxyViewModel> Proxies { get; private set; } = new();
        public ProxyViewModel? SelectedProxy { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void SourceCollectionChanged(object? sender, EventArgs e)
        {
            foreach (var viewModel in Sources)
            {
                viewModel.Dispose();
            }

            Sources = new ObservableCollection<SourceViewModel>(
                _miner.Producers.Items.Select(i => new SourceViewModel(i, _miner.Producers)));
            OnChanged(nameof(Sources));
        }

        private void ProxyCollectionChanged(object? sender, CollectionChangedEventArgs<Proxy> e)
        {
            var removedViewModels = new List<ProxyViewModel>();
            if (e.Action == CollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (var viewModel in Proxies.Where(vm => e.OldItems.Contains(vm.Proxy)))
                {
                    viewModel.Dispose();
                    removedViewModels.Add(viewModel);
                }
            }

            var remainViewModels = Proxies.Except(removedViewModels);
            var newViewModels = e.NewItems?.Select(i => new ProxyViewModel(i, _miner.Checker)) ?? Enumerable.Empty<ProxyViewModel>();

            Proxies = new ObservableCollection<ProxyViewModel>(remainViewModels.Concat(newViewModels));
            OnChanged(nameof(Proxies));
        }

        private void OnChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshStatus(object? sender, ElapsedEventArgs e)
        {
            ProxyStatus = $"Total proxies: {Proxies.Count}, " +
                $"checking: {Proxies.Count(p => p.StartCheck != null && p.FinishCheck == null)}, " +
                $"checked: {Proxies.Count(p => p.Status != null)}, " +
                $"valid: {Proxies.Count(p => p.IsValid != null && p.IsValid.Value)}, " +
                $"anonimous: {Proxies.Count(p => p.IsAnonimous != null && p.IsAnonimous.Value)}";
            OnChanged(nameof(ProxyStatus));
        }

        private static void SaveProxies(string filename, IEnumerable<Proxy> proxies)
        {
            File.WriteAllLines(filename, proxies.Select(p => string.Join(";", 
                p.Type.ToString(), p.Host, p.Port, p.Username, p.Password)));
        }
        
        private readonly IMiner _miner;
        private bool _isActive;
        private readonly Timer _refreshStatusTimer;
        private readonly TimeSpan _refreshPeriod = TimeSpan.FromSeconds(5);
    }
}