using WatcherCore;

namespace WatcherApp.ViewModels
{
    public class AddHostWindowViewModel
    {
        public WatchEntity Host { get; set; }

        public AddHostWindowViewModel()
        {
            Host = new WatchEntity();
            Host.Host = "localhost";
            Host.PingIntervalSeconds = 50;
        }
    }
}
