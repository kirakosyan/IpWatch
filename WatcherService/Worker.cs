using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WatcherCore;

namespace WatcherService
{
    public class Worker : BackgroundService
    {
        public static SMTPClient SmtpClient;
        private static ILogger Logger;
        //private readonly ILogger<Worker> _logger;
        private static IWatchRepo repo;
        private Dictionary<string, Thread> threads;
        private CancellationToken cancellationToken;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            Worker.Logger = logger;
            Worker.Logger.LogInformation("Folder: " + Directory.GetCurrentDirectory());
            //repo = new LocalDiskRepo.WatchRepo();
            repo = new LocalDbRepo.ListRepoContext();
            threads = new Dictionary<string, Thread>();

            var s = new SmtpSettings
            {
                EmailSender = configuration.GetValue<string>("SMTP:EmailSender"),
                Server = configuration.GetValue<string>("SMTP:Server"),
                Port = configuration.GetValue<int>("SMTP:Port"),
                Username = configuration.GetValue<string>("SMTP:Username"),
                SenderPassword = configuration.GetValue<string>("SMTP:Password")
            };

            SmtpClient = new SMTPClient(s);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Worker.Logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
            //SmtpClient.SendEmail("armen@kirakosyan.com", "test", "body test debug").GetAwaiter().GetResult();
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
                        Logger.LogInformation($"Buffer size: {reply.Buffer.Length}");

                        if (!entity.IsOnline)
                        {
                            entity.IsOnline = true;
                            await repo.Update(entity);
                            var r = await SmtpClient.SendHostStatusEmail(entity.Emails, true, entity.Host);
                            if(!r)
                            {
                                Logger.LogError($"Could not send email to {entity.Emails}");
                            }
                        }
                    }
                    else
                    {
                        if (entity.IsOnline)
                        {
                            entity.IsOnline = false;
                            await repo.Update(entity);
                            var r = await SmtpClient.SendHostStatusEmail(entity.Emails, false, entity.Host);
                            if (!r)
                            {
                                Logger.LogError($"Could not send email to {entity.Emails}");
                            }
                        }
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
                if (l.IsEnabled)
                {
                    var thread = new Thread(WatcherThread);
                    thread.Start(l);
                }
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
