using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.Infrastructure.Authorization
{
    // TODO: Implement authorization system, actually preferably in Application layer; if a good, simple use case can be established in this sample project.
    [ExcludeFromCodeCoverage]
    internal static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration _)
        {
        }
    }
}
