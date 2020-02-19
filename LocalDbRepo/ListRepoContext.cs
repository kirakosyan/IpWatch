using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using WatcherCore;

namespace LocalDbRepo
{
    public class ListRepoContext : DbContext
    {
        private string _connectionString;

        public DbSet<WatchEntity> WatchEntities { get; set; }

        public ListRepoContext(string connectionString = null)
        {
            if(string.IsNullOrWhiteSpace(connectionString))
            {
                _connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=IpWatcher;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            }
            else
            {
                _connectionString = connectionString;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
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
    }
}
