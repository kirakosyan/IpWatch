using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WatcherCore
{
    public interface IWatchRepo
    {
        string WatchListFileName { get; }
        Task<List<WatchEntity>> GetList();
        Task<bool> Insert(WatchEntity entity);
        Task<bool> Update(WatchEntity entity);
        Task<bool> Remove(Guid watchId);
        Task<WatchEntity> GetItem(List<WatchEntity> list, Guid watchId);
    }
}
