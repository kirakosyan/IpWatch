using System;

namespace WatcherCore
{
    public class WatchEntity
    {
        public Guid WatchId { get; set; }
        public string Host { get; set; }
        public int PingIntervalSeconds { get; set; }
        public string Emails { get; set; }
        public string Note { get; set; }
        public bool IsOnline { get; set; }
        public bool IsEnabled { get; set; }
    }
}
