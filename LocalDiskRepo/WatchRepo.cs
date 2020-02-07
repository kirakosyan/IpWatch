using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatcherCore;

namespace LocalDiskRepo
{
    public class WatchRepo : IWatchRepo
    {
        private string _settingsFileName = "WatchList.json";

        public string WatchListFileName { get { return _settingsFileName; } }

        public WatchRepo(string fileName = null)
        {
            if(!string.IsNullOrWhiteSpace(fileName))
            {
                _settingsFileName = fileName;
            }
        }

        public async Task<WatchListEntity> GetList()
        {
            if(!File.Exists(_settingsFileName))
            {
                return null;
            }
            var r = await File.ReadAllTextAsync(_settingsFileName, Encoding.UTF8);
            var list = JsonConvert.DeserializeObject<WatchListEntity>(r);
            return list;
        }

        public async Task<bool> Insert(WatchEntity entity)
        {
            var list = await GetList();
            if(list!=null)
            {
                if (list.WatchList.Any(e => { if (e.Host == entity.Host) { return true; } return false; }))
                {
                    return false;
                }
            }
            else
            {
                list = new WatchListEntity();
            }

            entity.WatchId = Guid.NewGuid();
            list.WatchList.Add(entity);
            await SaveList(list);

            return true;
        }

        public async Task<bool> Update(WatchEntity entity)
        {
            var list = await GetList();

            list.WatchList.Remove(entity);
            await SaveList(list);

            return true;
        }

        public async Task<bool> Remove(Guid watchId)
        {
            var list = await GetList();
            var item = await GetItem(list, watchId);
            list.WatchList.Remove(item);
            await SaveList(list);
            return true;
        }

        public async Task<WatchEntity> GetItem(WatchListEntity list, Guid watchId)
        {
            if(list == null)
            {
                list = await GetList();
            }

            if(list == null)
            {
                return null;
            }

            return list.WatchList.SingleOrDefault<WatchEntity>(e => { return e.WatchId == watchId; });
        }

        private async Task SaveList(WatchListEntity list)
        {
            await File.WriteAllTextAsync(_settingsFileName, JsonConvert.SerializeObject(list), Encoding.UTF8);
        }
    }
}
