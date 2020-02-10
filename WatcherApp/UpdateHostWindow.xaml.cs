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
using WatcherCore;

namespace WatcherApp
{
    /// <summary>
    /// Interaction logic for UpdateHostWindow.xaml
    /// </summary>
    public partial class UpdateHostWindow : Window
    {
        public UpdateHostWindow(WatchEntity entity)
        {
            InitializeComponent();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
