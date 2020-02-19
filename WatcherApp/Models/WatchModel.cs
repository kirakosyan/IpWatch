using WatcherCore;

namespace WatcherApp.Models
{
    public class WatchModel : WatchEntity
    {
        public byte[] StatusImage
        {
            get
            {
                return IsOnline ? null : WatcherResources.red_dot;
            }
        }
    }
}
