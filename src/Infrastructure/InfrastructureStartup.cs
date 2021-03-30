using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MyWarehouse.Infrastructure.UnitTests")]

namespace MyWarehouse.Infrastructure
{
    // This class implements a rather crude modular configuration of subcomponents, without any ceremony or meta-structure.
    // Proper abstractions can be added later if modularization would seem to benefit from them.

    [ExcludeFromCodeCoverage]
    public static class InfrastructureStartup
    {
        public static void AddMyInfrastructureConfiguration(this IConfigurationBuilder configBuilder, HostBuilderContext context)
        {
            configBuilder.AddJsonFile("infrastructureSettings.json", optional: true);

            AzureKeyVault.Startup.ConfigureAppConfiguration(context, configBuilder);
        }

        public static void AddMyInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            Identity.Startup.ConfigureServices(services, configuration);
            Authentication.Startup.ConfigureServices(services, configuration);
            Persistence.Startup.ConfigureServices(services, configuration, env);
            ApplicationDependencies.Startup.ConfigureServices(services, configuration);
        }

        public static void UseMyInfrastructure(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env)
        {
            Authentication.Startup.Configure(app);
            Persistence.Startup.Configure(app, configuration);
        }
    }
}
