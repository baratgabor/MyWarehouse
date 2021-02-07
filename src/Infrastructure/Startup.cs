using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MyWarehouse.Infrastructure.UnitTests")]

namespace MyWarehouse.Infrastructure
{
    // This class implements a rather crude modular configuration of subcomponents, without any ceremony or meta-structure.
    // Proper abstractions can be added later if modularization would seem to benefit from them.

    public static class Startup
    {
        public static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder configBuilder)
        {
            configBuilder.AddJsonFile("infrastructureSettings.json", optional: true);

            AzureKeyVault.Startup.ConfigureAppConfiguration(context, configBuilder);
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            Swagger.Startup.ConfigureServices(services, configuration);
            Identity.Startup.ConfigureServices(services, configuration);
            Authentication.Startup.ConfigureServices(services, configuration);
            Persistence.Startup.ConfigureServices(services, configuration, env);
            ApplicationDependencies.Startup.ConfigureServices(services, configuration);

            return services;
        }

        public static void Configure(IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env)
        {
            Authentication.Startup.Configure(app);
            Persistence.Startup.Configure(app, configuration);
            Swagger.Startup.Configure(app, configuration);
        }
    }
}
