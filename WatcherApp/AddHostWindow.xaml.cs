using System;
using System.Windows;
using WatcherApp.ViewModels;

namespace WatcherApp
{
    /// <summary>
    /// Interaction logic for AddHostWindow.xaml
    /// </summary>
    public partial class AddHostWindow : Window
    {
        AddHostWindowViewModel viewModel { get; set; }

        public AddHostWindow()
        {
            InitializeComponent();

            viewModel = new AddHostWindowViewModel();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            viewModel.Host.IsEnabled = true;

            DataContext = viewModel;
        }

        private async void Insert_Click(object sender, RoutedEventArgs e)
        {
            await App.Repo.Insert(viewModel.Host);
            this.Close();
        }
    }
}
