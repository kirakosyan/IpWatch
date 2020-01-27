using System;
using System.Threading.Tasks;

namespace WatcherCore
{
    public interface IWatchRepo
    {
        Task<WatchListEntity> GetList();
        Task<bool> Insert(WatchEntity entity);
        Task<bool> Update(WatchEntity entity);
        Task<bool> Remove(Guid watchId);
        Task<WatchEntity> GetItem(WatchListEntity list, Guid watchId);
    }
}
