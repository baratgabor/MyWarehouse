using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWarehouse.Infrastructure;
using MyWarehouse.WebApi.CORS.Settings;

namespace MyWarehouse.WebApi.CORS
{
    public static class CorsStartup
    {
        public static void AddMyCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSettings = configuration.GetMyOptions<CorsSettings>();

            if (corsSettings == null)
                return;

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins(
                        corsSettings.AllowedOrigins)
                    .Build();
                });
            });
        }

        public static void UseMyCorsConfiguration(this IApplicationBuilder app)
        {
            app.UseCors();
        }
    }
}
