using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using MyWarehouse.Application.Dependencies.Services;
using MyWarehouse.WebApi.Authentication.Services;
using MyWarehouse.WebApi.Logging;
using MyWarehouse.WebApi.ErrorHandling;
using System.Text.Json;
using MyWarehouse.WebApi.CORS;
using MyWarehouse.Infrastructure;

namespace MyWarehouse
{
    public class Startup
    {
        protected IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AddMyOptions(services);
            AddMyControllers(services);
            AddMyApiServices(services);

            services.AddHealthChecks();

            Infrastructure.Startup.ConfigureServices(services, Configuration, Environment);
            Application.Startup.ConfigureServices(services);

            AddMyCorsConfig(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

#if DEBUG
            app.Use(async (ctx, next) =>{

                // Break here to debug HttpContext, Request, or Response.
                await next();
            });
#endif

            app.AddMyRequestLogging();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors();

            Infrastructure.Startup.Configure(app, Configuration, Environment);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        private void AddMyCorsConfig(IServiceCollection services)
        {

            var corsSettings = Configuration.GetMyOptions<CorsSettings>();

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

        /// <summary>
        /// Binds strongly typed option classes to the aggregate configuration.
        /// Classes configured here are injectable into components by the IoC container via IOptions<className>.
        /// </summary>
        protected virtual void AddMyOptions(IServiceCollection services)
        {
            services.AddOptions();

            // Add API-related strongly typed options here.
            services.RegisterMyOptions<CorsSettings>();
        }

        protected virtual void AddMyControllers(IServiceCollection services)
        {
            services.AddControllers(
                options => options.Filters.Add<ApiExceptionFilter>()
            )
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(c => 
                    c.JsonSerializerOptions.PropertyNamingPolicy 
                        = JsonNamingPolicy.CamelCase); // Supposed to be default, but just to make sure.
        }

        protected virtual void AddMyApiServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }
    }
}
