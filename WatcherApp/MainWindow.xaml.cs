using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;
using WatcherApp.ViewModels;
using WatcherCore;

namespace WatcherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel();
            var t = Task.Run(() => { viewModel.LoadList(); });
            t.Wait();
            this.DataContext = viewModel;
            //MessageBox.Show("count: " + viewModel.List?.WatchList?.Count);
        }

        private async void AddHost_Click(object sender, RoutedEventArgs e)
        {
            var addHost = new AddHostWindow();
            addHost.ShowDialog();

            await viewModel.LoadList();
            //WatchList.Rebind();
            //MessageBox.Show("count: " + viewModel.List?.Count);
        }

        private void DeleteHost_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelectedItem == null)
            {
                RadWindow.Alert("Please select host first.");
                return;
            }
            //viewModel.List.WatchList.RemoveAt(0);
            RadWindow.Confirm($"Are you sure you want to delete {viewModel.SelectedItem.Host}?", this.OnDeleteConfirmClosed);
        }

        private async void OnDeleteConfirmClosed(object sender, WindowClosedEventArgs e)
        {
            var result = e.DialogResult;
            if (result == true)
            {
                await viewModel.Delete(viewModel.SelectedItem.WatchId);
                //WatchList.Rebind();
            }
        }

        private void WatchList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var updaeHost = new UpdateHostWindow(viewModel.SelectedItem);
            updaeHost.ShowDialog();
        }
    }
}
