using MyWarehouse.Application.Dependencies.Services;
using MyWarehouse.WebApi.Authentication.Services;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.WebApi.Authentication;

[ExcludeFromCodeCoverage]
internal static class AuthenticationStartup
{
    public static void AddMyApiAuthDeps(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
    }
}
