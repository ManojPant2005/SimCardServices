using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;

namespace SIMCardOfferService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration;
                    var connectionString = config.GetConnectionString("DefaultConnection");
                    var tableName = config["DatabaseConfiguration:TableName"];
                    var databaseType = config["DatabaseConfiguration:DatabaseType"];

                    // Register NotificationService with the appropriate constructor arguments
                    services.AddHostedService(provider =>
                        new NotificationService(
                            provider.GetRequiredService<ILogger<NotificationService>>(),
                            connectionString,
                            tableName,
                            databaseType));
                });
    }
}