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

            DataContext = viewModel;
        }
    }
}
