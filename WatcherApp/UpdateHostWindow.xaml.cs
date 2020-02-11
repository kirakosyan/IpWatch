using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
