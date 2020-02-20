using LocalDbRepo;
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
            Repo = new ListRepoContext();
        }
    }
}
