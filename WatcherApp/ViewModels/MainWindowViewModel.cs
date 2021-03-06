﻿using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Timers;
using WatcherApp.Models;

namespace WatcherApp.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<WatchModel> List { get; set; }
        public WatchModel SelectedItem { get; set; }

        private Timer timer;

        public MainWindowViewModel()
        {
            List = new ObservableCollection<WatchModel>();

            timer = new Timer();
            timer.Interval = 10000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            var list = await App.Repo.GetList();
            for (int c = 0; c < list.Count; c++)
            {
                string hash1;
                string hash2;
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    hash1 = Convert.ToBase64String(sha1.ComputeHash(list[c].Timestamp));
                    hash2 = Convert.ToBase64String(sha1.ComputeHash(List[c].Timestamp));
                }

                if (hash1 != hash2)
                {
                    await LoadList();
                    timer.Start();
                    break;
                }
            }

            timer.Start();
        }

        public async Task LoadList()
        {
            var list = await App.Repo.GetList();
            List.Clear();
            foreach (var l in list)
            {
                List.Add(new WatchModel
                {
                    WatchId = l.WatchId,
                    Emails = l.Emails,
                    Host = l.Host,
                    IsEnabled = l.IsEnabled,
                    IsOnline = l.IsOnline,
                    Note = l.Note,
                    PingIntervalSeconds = l.PingIntervalSeconds,
                    Timestamp = l.Timestamp,
                    TimeSinceLastStatusChange = l.TimeSinceLastStatusChange
                });
            }
        }

        public async Task Delete(Guid watchId)
        {
            await App.Repo.Remove(watchId);
            await LoadList();
        }
    }
}
