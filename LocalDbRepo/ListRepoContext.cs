using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using WatcherCore;

namespace LocalDbRepo
{
    public class ListRepoContext : DbContext, IWatchRepo
    {
        private string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=IpWatcher;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        public DbSet<WatchEntity> WatchEntities { get; set; }

        public ListRepoContext(string connectionString = null)
        {
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                ConnectionString = connectionString;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }

        public static string GenerateUniqueDatabaseName(string originialConnectionString)
        {
            SqlConnectionStringBuilder connectionInfo = new SqlConnectionStringBuilder(originialConnectionString);
            connectionInfo.InitialCatalog = string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1}",
                connectionInfo.InitialCatalog,
                Guid.NewGuid());

            return connectionInfo.ToString();
        }

        public Task<List<WatchEntity>> GetList()
        {
            return WatchEntities.AsNoTracking().ToListAsync();
        }

        public async Task<bool> Insert(WatchEntity entity)
        {
            entity.WatchId = Guid.NewGuid();
            await WatchEntities.AddAsync(entity);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> Update(WatchEntity entity)
        {
            var c = await this.Database.ExecuteSqlInterpolatedAsync($@"update WatchEntities set 
Emails ={entity.Emails},
Host ={entity.Host},
IsOnline ={entity.IsOnline},
IsEnabled ={entity.IsEnabled},
PingIntervalSeconds ={entity.PingIntervalSeconds},
Note = {entity.Note}
where WatchId = {entity.WatchId}");
            
            return c == 1;
        }

        public async Task<bool> Remove(Guid watchId)
        {
            //var e = await GetItem(watchId);
            // WatchEntities.Remove(e);
            using (var context = new ListRepoContext(ConnectionString))
            {
                context.Remove<WatchEntity>(new WatchEntity { WatchId = watchId });
                await context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<WatchEntity> GetItem(Guid watchId)
        {
            var e = await WatchEntities.FromSqlInterpolated($@"select * from WatchEntities where WatchId = {watchId}").FirstOrDefaultAsync();
            return e;
        }
    }
}
