using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace MyWarehouse.WebApi.Authentication;

[ExcludeFromCodeCoverage]
internal static class ApiStartup
{
    public static void AddMyApi(this IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddControllers()
            .AddControllersAsServices()
            .AddJsonOptions(c =>
                c.JsonSerializerOptions.PropertyNamingPolicy
                    = JsonNamingPolicy.CamelCase); // Supposed to be default, but just to make sure.
    }

    /// <summary>
    /// Depends on UseRouting() being called before calling this method.
    /// </summary>
    public static void UseMyApi(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}
