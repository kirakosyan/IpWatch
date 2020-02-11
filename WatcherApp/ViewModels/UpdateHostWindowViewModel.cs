using WatcherCore;

namespace WatcherApp.ViewModels
{
    public class UpdateHostWindowViewModel
    {
        public UpdateHostWindowViewModel(WatchEntity entity = null)
        {
            Host = new WatchEntity();

            if(entity != null)
            {
                Host.WatchId = entity.WatchId;
                Host.Host = entity.Host;
                Host.Emails = entity.Emails;
                Host.PingIntervalSeconds = entity.PingIntervalSeconds;
                Host.IsEnabled = entity.IsEnabled;
                Host.Note = entity.Note;
            }
        }

        public WatchEntity Host { get; set; }
    }
}
