using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Kaspirin.UI.Framework.Threading;
using ProxyMiner.Checkers;
using ProxyMiner.Core;
using ProxyMiner.Core.Models;
using ProxyMiner.Core.Models.BaseCollections;
using ProxyMiner.Core.Options;
using ProxyMiner.Core.Producers;
using ProxyMiner.Providers;
using ProxyMiner.Providers.CsvFile;
using ProxyMiner.Providers.FreeProxyList;
using ProxyMiner.Providers.GeoNode;
using Timer = System.Timers.Timer;
using CollectionChangeAction = ProxyMiner.Core.Models.BaseCollections.CollectionChangeAction;

namespace ProxyMiner.Demo.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        var checker = new Checker();
        _settingsProvider = new JsonSettingsProvider("options.json");
        
        _miner = MinerFactory.GetMiner(checker, _settingsProvider);

        _miner.Producers.CollectionChanged += SourceCollectionChanged;
        _miner.Producers.AddRange(
        [
            new Producer("Free-Proxy-List", new FreeProxyListProvider()),
            new Producer("all.csv", new CsvFileProvider("all.csv", CsvFileSettings.Default)),
            new Producer("valid.csv", new CsvFileProvider("valid.csv", CsvFileSettings.Default)),
            new Producer("anonimous.csv", new CsvFileProvider("anonimous.csv", CsvFileSettings.Default)),
            new Producer("GeoNode", new GeoNodeProvider()),
            new Producer("Dummy", new DummyProvider())
         ]);

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
            () => _miner.Producers.Add(new Producer("all.csv", new CsvFileProvider("all.csv", CsvFileSettings.Default))));
        RemoveSourceCommand = new RelayCommand(
            () => _miner.Producers.Remove(_miner.Producers.Items.Last()),
            () => _miner.Producers.Items.Any());
     
        SaveAllProxies = new RelayCommand(
            () => SaveProxies("all.csv", _miner.Proxies.Items), 
            () => _miner.Proxies.Items.Any());
        SaveValidProxies = new RelayCommand(
            () => SaveProxies("valid.csv", _miner.ProxyFilter.Apply(modifier => modifier.Valid(true))), 
            () => _miner.Proxies.Items.Any());
        SaveAnonimousProxies = new RelayCommand(
            () => SaveProxies("anonimous.csv", _miner.ProxyFilter.Apply(modifier => modifier.Anonimous(true))), 
            () => _miner.Proxies.Items.Any());
        
        CheckSelectedProxies = new RelayCommand(
            () => _miner.Checker.CheckNow([SelectedProxy!.Proxy]),
            () => SelectedProxy != null);
        StopCheckingSelectedProxies = new RelayCommand(
            () => _miner.Checker.StopChecking([SelectedProxy!.Proxy]),
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
    public ICommand StopCheckingSelectedProxies { get; }
    
    public string? ProxyStatus { get; private set; }

    public ObservableCollection<SourceViewModel> Sources { get; } = [];
    public ObservableCollection<ProxyViewModel> Proxies { get; } = [];
    public ProxyViewModel? SelectedProxy { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SourceCollectionChanged(object? sender, CollectionChangedEventArgs<Producer> e)
    {
        Executers.InUiAsync(() =>
        {
            var newItems = e.NewItems?.Select(i => new SourceViewModel(i, _miner.Producers)).ToList() ?? [];
            var removedItems = e.Action == CollectionChangeAction.Remove && e.OldItems != null
                ? Sources.Where(vm => e.OldItems.Contains(vm.Producer)).ToList()
                : [];

            newItems.ForEach(item => Sources.Add(item));
            removedItems.ForEach(item =>
            {
                item.Dispose();
                Sources.Remove(item);
            });
        });
    }

    private void ProxyCollectionChanged(object? sender, CollectionChangedEventArgs<Proxy> e)
    {
        Executers.InUiAsync(() =>
        {
            var newItems = e.NewItems?.Select(i => new ProxyViewModel(i, _miner.Checker)).ToList() ?? [];
            var removedItems = e.Action == CollectionChangeAction.Remove && e.OldItems != null
                ? Proxies.Where(vm => e.OldItems.Contains(vm.Proxy)).ToList()
                : [];

            newItems.ForEach(Proxies.Add);
            removedItems.ForEach(item =>
            {
                item.Dispose();
                Proxies.Remove(item);
            });
        });
    }

    private void OnChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RefreshStatus(object? sender, ElapsedEventArgs e)
    {
        Task.Run(() =>
        {
            var totalProxies = Proxies.Count;
            var checking = 0;
            var checkedProxies = 0;
            var valid = 0;
            var anonymous = 0;

            foreach (var proxy in Proxies)
            {
                if (proxy.StartCheckUtc != null && proxy.FinishCheckUtc == null)
                    checking++;
                if (proxy.Status != null)
                    checkedProxies++;
                if (proxy.IsValid == true)
                    valid++;
                if (proxy.IsAnonimous == true)
                    anonymous++;
            }

            var status = $"Total proxies: {totalProxies}, " +
                $"checking: {checking}, " +
                $"checked: {checkedProxies}, " +
                $"valid: {valid}, " +
                $"anonimous: {anonymous}";

            Executers.InUiAsync(() =>
            {
                ProxyStatus = status;
                OnChanged(nameof(ProxyStatus));
            });
        });
    }

    private static void SaveProxies(string filename, IEnumerable<Proxy> proxies)
    {
        File.WriteAllLines(filename, proxies.Select(p => string.Join(";", 
            p.Type.ToString(), p.Host, p.Port, 
            p.AuthorizationData?.Username, p.AuthorizationData?.Password)));
    }
    
    private readonly IMiner _miner;
    private bool _isActive;
    private readonly Timer _refreshStatusTimer;
    private readonly TimeSpan _refreshPeriod = TimeSpan.FromSeconds(5);
    private readonly JsonSettingsProvider _settingsProvider;
}