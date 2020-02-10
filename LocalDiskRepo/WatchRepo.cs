using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public async Task<List<WatchEntity>> GetList()
        {
            if(!File.Exists(_settingsFileName))
            {
                return null;
            }
            var r = await File.ReadAllTextAsync(_settingsFileName, Encoding.UTF8);
            var list = JsonConvert.DeserializeObject<List<WatchEntity>>(r);
            return list;
        }

        public async Task<bool> Insert(WatchEntity entity)
        {
            var list = await GetList();
            if(list!=null)
            {
                if (list.Any(e => { if (e.Host == entity.Host) { return true; } return false; }))
                {
                    return false;
                }
            }
            else
            {
                list = new List<WatchEntity>();
            }

            entity.WatchId = Guid.NewGuid();
            list.Add(entity);
            await SaveList(list);

            return true;
        }

        public async Task<bool> Update(WatchEntity entity)
        {
            var list = await GetList();

            list.Remove(entity);
            await SaveList(list);

            return true;
        }

        public async Task<bool> Remove(Guid watchId)
        {
            var list = await GetList();
            var item = await GetItem(list, watchId);
            list.Remove(item);
            await SaveList(list);
            return true;
        }

        public async Task<WatchEntity> GetItem(List<WatchEntity> list, Guid watchId)
        {
            if(list == null)
            {
                list = await GetList();
            }

            if(list == null)
            {
                return null;
            }

            return list.SingleOrDefault<WatchEntity>(e => { return e.WatchId == watchId; });
        }

        private async Task SaveList(List<WatchEntity> list)
        {
            await File.WriteAllTextAsync(_settingsFileName, JsonConvert.SerializeObject(list), Encoding.UTF8);
        }
    }
}
