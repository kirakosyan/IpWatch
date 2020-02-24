using System;
using System.Windows;
using WatcherApp.ViewModels;
using WatcherCore;

namespace WatcherApp
{
    /// <summary>
    /// Interaction logic for UpdateHostWindow.xaml
    /// </summary>
    public partial class UpdateHostWindow : Window
    {
        private UpdateHostWindowViewModel viewModel;

        public UpdateHostWindow(WatchEntity entity)
        {
            InitializeComponent();
            viewModel = new UpdateHostWindowViewModel(entity);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            DataContext = viewModel;
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            await App.Repo.Update(viewModel.Host);
            this.Close();
        }
    }
}
