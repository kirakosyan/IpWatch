using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                Logger.LogInformation($"Thread {entity.Host} ping");
                await Task.Delay(entity.PingIntervalSeconds * 1000);
            }
            //var t = Task.Run(async () => {
            //    _logger.LogInformation($"Starting thread 1");
            //    while (true)
            //    {
            //        _logger.LogInformation($"Thread 1 ping");
            //        await Task.Delay(10000, cancellationToken);
            //    }

            //}, cancellationToken);
            //_logger.LogInformation("Thread status: " + t.Status);
            //if(t.Status == TaskStatus.Canceled)
            //{
            //    t.Wait();
            //}

            //tasks.Add("1", t);
            //_logger.LogInformation($"Thread 1 started");
        }

        private async void StartWatching()
        {
            Worker.Logger.LogInformation("Worker executing at: {time}", DateTimeOffset.Now);

            var list = await repo.GetList();
            foreach(var l in list?.WatchList)
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
