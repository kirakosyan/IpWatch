using LocalDiskRepo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WatcherCore;

namespace WatcherApp.ViewModels
{
    public class MainWindowViewModel
    {
        IWatchRepo repo;
        public WatchListEntity List { get; set; }
        
        public MainWindowViewModel()
        {
            repo = new WatchRepo();
        }

        public async Task LoadList()
        {
            List = await repo.GetList();
        }
    }
}
