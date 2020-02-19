using System;
using System.ComponentModel.DataAnnotations;

namespace WatcherCore
{
    public class WatchEntity
    {
        [Key]
        public Guid WatchId { get; set; }
        public string Host { get; set; }
        public int PingIntervalSeconds { get; set; }
        public string Emails { get; set; }
        public string Note { get; set; }
        public bool IsOnline { get; set; }
        public DateTime TimeSinceLastStatusChange { get; set; }
        public bool IsEnabled { get; set; }
    }
}
