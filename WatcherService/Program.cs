using LocalDbRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

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

            CreateHostBuilder(args)
                .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                configApp.SetBasePath(Directory.GetCurrentDirectory());
                configApp.AddJsonFile("appsettings.json", optional: true);
#if DEBUG
                configApp.AddJsonFile(
                    $"appsettings.Development.json",
                    optional: false);
#else
                configApp.AddJsonFile(
                    $"appsettings.Release.json",
                    optional: false);
#endif
                configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                configApp.AddCommandLine(args);
            })
                .Build().Run();
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
