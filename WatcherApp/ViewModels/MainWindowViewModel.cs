using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WatcherCore;

namespace WatcherApp.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<WatchEntity> List { get; set; }
        public WatchEntity SelectedItem { get; set; }

        public MainWindowViewModel()
        {
            List = new ObservableCollection<WatchEntity>();
        }

        public async Task LoadList()
        {
            var list = await App.Repo.GetList();
            List.Clear();
            foreach(var l in list)
            {
                List.Add(l);
            }
        }

        public async Task Delete(Guid watchId)
        {
            await App.Repo.Remove(watchId);
            await LoadList();
        }
    }
}
