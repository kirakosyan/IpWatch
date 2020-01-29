using LocalDiskRepo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WatcherCore;

namespace WatcherApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IWatchRepo Repo;

        public App()
        {
            Repo = new WatchRepo();
        }
    }
}
