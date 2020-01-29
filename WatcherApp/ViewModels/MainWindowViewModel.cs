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
        
        public WatchListEntity List { get; set; }
        
        public MainWindowViewModel()
        {
            List = new WatchListEntity();
        }

        public async Task LoadList()
        {
            List = await App.Repo.GetList();
        }
    }
}
