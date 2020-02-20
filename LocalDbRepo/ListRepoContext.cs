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
            if(!string.IsNullOrWhiteSpace(connectionString))
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
            return WatchEntities.ToListAsync();
        }

        public async Task<bool> Insert(WatchEntity entity)
        {
            entity.WatchId = Guid.NewGuid();
            await WatchEntities.AddAsync(entity);
            await SaveChangesAsync();
            return true;
        }

        public new Task<bool> Update(WatchEntity entity)
        {
            throw new NotImplementedException();
        }

        public new Task<bool> Remove(Guid watchId)
        {
            throw new NotImplementedException();
        }

        public Task<WatchEntity> GetItem(List<WatchEntity> list, Guid watchId)
        {
            return WatchEntities.FirstOrDefaultAsync(i=>i.WatchId == watchId); 
        }
    }
}
