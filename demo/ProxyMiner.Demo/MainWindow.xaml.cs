using System.Windows;
using System.Windows.Data;
using ProxyMiner.Demo.ViewModels;

namespace ProxyMiner.Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void Proxies_Filter(object sender, FilterEventArgs e)
        {
            //throw new System.NotImplementedException();
        }
    }
}
