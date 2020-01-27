using System.Collections.Generic;

namespace WatcherCore
{
    public class WatchListEntity
    {
        public WatchListEntity()
        {
            WatchList = new List<WatchEntity>();
        }

        public List<WatchEntity> WatchList { get; set; }
    }
}
