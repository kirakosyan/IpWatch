using LocalDbRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// https://medium.com/@nickfane/introduction-to-worker-services-in-net-core-3-0-4bb3fc631225
namespace WatcherService
{
    public class Program
    {
        private static ListRepoContext listRepoContext;
        public static void Main(string[] args)
        {
            listRepoContext = new ListRepoContext();
            listRepoContext.Database.Migrate();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
