using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
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
        private static IWatchRepo repo;
        private Dictionary<Guid, Thread> threads;
        private CancellationToken cancellationToken;
        private static List<WatchEntity> EntityList;

        private System.Timers.Timer timer;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            Worker.Logger = logger;
            Worker.Logger.LogInformation("Folder: " + Directory.GetCurrentDirectory());

            repo = new LocalDbRepo.ListRepoContext();
            threads = new Dictionary<Guid, Thread>();

            var s = new SmtpSettings
            {
                EmailSender = configuration.GetValue<string>("SMTP:EmailSender"),
                Server = configuration.GetValue<string>("SMTP:Server"),
                Port = configuration.GetValue<int>("SMTP:Port"),
                Username = configuration.GetValue<string>("SMTP:Username"),
                SenderPassword = configuration.GetValue<string>("SMTP:Password")
            };

            SmtpClient = new SMTPClient(s);

            timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += Timer_Elapsed; ;
            timer.Start();
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //timer.Stop();

            //var list = await repo.GetList();
            //for (int c = 0; c < list.Count; c++)
            //{
            //    string hash1;
            //    string hash2;
            //    using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            //    {
            //        hash1 = Convert.ToBase64String(sha1.ComputeHash(list[c].Timestamp));
            //        hash2 = Convert.ToBase64String(sha1.ComputeHash(List[c].Timestamp));
            //    }

            //    if (hash1 != hash2)
            //    {
            //        await LoadList();
            //        timer.Start();
            //        break;
            //    }
            //}

            //timer.Start();
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
                // var e = await repo.GetItem(entity.WatchId);
                var e = EntityList.Find(m => { return m.WatchId == entity.WatchId; });
                if(!e.IsEnabled)
                {
                    Worker.Logger.LogInformation($"{entity.Host} was disabled");
                    break;
                }

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

            EntityList = await repo.GetList();
            threads = new Dictionary<Guid, Thread>();
            foreach(var l in EntityList)
            {
                if (l.IsEnabled)
                {
                    var thread = new Thread(WatcherThread);
                    
                    thread.Start(l);
                    threads.Add(l.WatchId, thread);
                }
            }

            EntityList = await repo.GetList();
            while (!cancellationToken.IsCancellationRequested)
            {
                var list = await repo.GetList();
                for (int c = 0; c < list.Count; c++)
                {
                    string hash1;
                    string hash2;
                    using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                    {
                        hash1 = Convert.ToBase64String(sha1.ComputeHash(list[c].Timestamp));
                        hash2 = Convert.ToBase64String(sha1.ComputeHash(EntityList[c].Timestamp));
                    }

                    if (hash1 != hash2)
                    {
                        if (threads.ContainsKey(list[c].WatchId))
                        {
                            threads.Remove(list[c].WatchId);
                        }

                        if (list[c].IsEnabled)
                        {
                            var thread = new Thread(WatcherThread);

                            EntityList = await repo.GetList();
                            
                            thread.Start(list[c]);
                            threads.Add(list[c].WatchId, thread);

                            Logger.LogInformation($"{list[c].Host} host was updated, thread was restarted.");
                        }
                        else
                        {
                            Logger.LogInformation($"{list[c].Host} host was updated, not pinging anymore.");
                        }
                    }
                }

                EntityList = await repo.GetList();

                Logger.LogInformation("Checking host status completed");
                await Task.Delay(10000);
            }
        }
    }
}
