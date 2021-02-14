using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWarehouse.Infrastructure.Authorization.Constants;

namespace MyWarehouse.Infrastructure.Authorization
{
    // TODO: Implement authorization system, actually preferably in Application layer; if a good, simple use case can be established in this sample project.
    internal static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration _)
        {
        }
    }
}
