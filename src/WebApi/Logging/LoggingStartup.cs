using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MyWarehouse.WebApi.Logging.Helper;
using MyWarehouse.WebApi.Logging.Settings;
using MyWarehouse.Infrastructure;
using Serilog;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.WebApi.Logging
{
    [ExcludeFromCodeCoverage]
    internal static class LoggingStartup
    {
        public static IWebHostBuilder AddMySerilogLogging(this IWebHostBuilder webBuilder)
        {
            return webBuilder.UseSerilog((context, loggerCfg) =>
            {
                loggerCfg
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("EnvironmentName", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithMachineName();

                if (context.HostingEnvironment.IsDevelopment())
                {
                    loggerCfg
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Event} - {Message}{NewLine}{Exception}")
                    .WriteTo.Debug();
                }
                else
                {
                    loggerCfg
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Error);
                }

                var logglySettings = context.Configuration.GetMyOptions<LogglySettings>();
                if (logglySettings.WriteToLoggly.GetValueOrDefault() == true)
                {
                    loggerCfg.WriteTo.Loggly(
                        customerToken: logglySettings.CustomerToken);
                }
            });
        }

        /// <summary>
        /// Adds Serilog request logging to the request processing pipeline.
        /// Call it early in the pipeline to capture as much as possible.
        /// </summary>
        public static IApplicationBuilder UseMyRequestLogging(this IApplicationBuilder appBuilder)
        {
            return appBuilder
                // Log requests
                .UseSerilogRequestLogging(
                    // Don't log health check endpoints
                    opts => opts.GetLevel = LogHelper.ExcludeHealthChecks);
        }
    }
}