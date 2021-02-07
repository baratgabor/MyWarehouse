using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MyWarehouse.WebApi.Logging;
using System.Reflection;

namespace MyWarehouse
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    webBuilder => {
                        // Notice: Logging overrides.
                        webBuilder.AddMySerilogLogging(); 

                        webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    // ConfigureWebHostDefaults only adds secrets if environment is Develop.
                    // This ensures they're always added, for local testing of Production setting.
                    config.AddUserSecrets(Assembly.GetEntryAssembly(), optional: true);

                    // Notice: Infrastructure hook.
                    Infrastructure.Startup.ConfigureAppConfiguration(context, config);
                });
    }
}