using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace MyWarehouse.WebApi.Authentication
{
    public static class ApiStartup
    {
        public static void AddMyApi(this IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers()
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
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
}
