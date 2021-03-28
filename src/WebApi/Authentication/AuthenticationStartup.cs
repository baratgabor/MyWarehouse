using Microsoft.Extensions.DependencyInjection;
using MyWarehouse.Application.Dependencies.Services;
using MyWarehouse.WebApi.Authentication.Services;

namespace MyWarehouse.WebApi.Authentication
{
    public static class AuthenticationStartup
    {
        public static void AddMyApiAuthDeps(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }
    }
}
