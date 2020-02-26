using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// dotnet tool install --global dotnet-ef --version 3.1.1
namespace LocalDbRepo.Tests
{
    public abstract class TestBase
    {
        protected static ListRepoContext RepoContext { get; set; }
    }

    [TestClass]
    public class Init : TestBase
    {
        private static string connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=IpWatcherTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        [AssemblyInitialize]
        public static void SetupDb(TestContext context)
        {
            RepoContext = new ListRepoContext(ListRepoContext.GenerateUniqueDatabaseName(connectionString));
            RepoContext.Database.Migrate();
        }

        [AssemblyCleanup]
        public static void TearDown()
        {
            RepoContext.Database.EnsureDeleted();
        }
    }
}
