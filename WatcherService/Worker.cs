using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WatcherCore;

namespace WatcherService
{
    public class Worker : BackgroundService
    {
        private static ILogger Logger;
        //private readonly ILogger<Worker> _logger;
        private IWatchRepo repo;
        private Dictionary<string, Thread> threads;
        private FileSystemWatcher fileWatcher;
        private CancellationToken cancellationToken;

        public Worker(ILogger<Worker> logger)
        {
            Worker.Logger = logger;
            Worker.Logger.LogInformation("Folder: " + Directory.GetCurrentDirectory());
            repo = new LocalDiskRepo.WatchRepo();
            threads = new Dictionary<string, Thread>();

            fileWatcher = new FileSystemWatcher(Directory.GetCurrentDirectory());
            fileWatcher.Filter = repo.WatchListFileName;
            fileWatcher.Changed += FileWatcher_Changed;
            // Begin watching.
            fileWatcher.EnableRaisingEvents = true;
        }

        private async void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            fileWatcher.EnableRaisingEvents = false;
            Worker.Logger.LogInformation("Restarting Watcher!");
            await this.StopAsync(cancellationToken);
            foreach(var t in threads)
            {
                t.Value.Abort();
            }

            threads.Clear();
            Thread.Sleep(2000);
            cancellationToken = new CancellationToken(false);
            StartWatching();
            fileWatcher.EnableRaisingEvents = true;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Worker.Logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            StartWatching();
        }

        private static async void WatcherThread(object obj)
        {
            var entity = (WatchEntity)obj;
            Worker.Logger.LogInformation("Starting thread for " + entity.Host);
            while (true)
            {
                string data = "a quick brown fox jumped over the lazy dog";

                Ping pingSender = new Ping();
                PingOptions options = new PingOptions
                {
                    DontFragment = true
                };

                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1024;

                Logger.LogInformation($"Pinging {entity.Host}");
                try
                {
                    PingReply reply = pingSender.Send(entity.Host, timeout, buffer, options);
                    if (reply.Status == IPStatus.Success)
                    {
                        Logger.LogInformation($"Address: {reply.Address}");
                        Logger.LogInformation($"RoundTrip time: {reply.RoundtripTime}");
                        //Logger.LogInformation($"Time to live: {reply.Options?.Ttl}");
                        //Logger.LogInformation($"Don't fragment: {reply.Options?.DontFragment}");
                        Logger.LogInformation($"Buffer size: {reply.Buffer.Length}");
                    }
                    else
                    {
                        Logger.LogError(reply.Status.ToString());
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }

                await Task.Delay(entity.PingIntervalSeconds * 1000);
            }
        }

        private async void StartWatching()
        {
            Worker.Logger.LogInformation("Worker executing at: {time}", DateTimeOffset.Now);

            var list = await repo.GetList();
            foreach(var l in list)
            {
                var thread = new Thread(WatcherThread);;
                thread.Start(l);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                //Worker.Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                list = await repo.GetList();
                //Worker.Logger.LogInformation($"Found {list?.WatchList?.Count ?? 0} hosts to ping");

                await Task.Delay(10000);
            }
        }
    }
}
