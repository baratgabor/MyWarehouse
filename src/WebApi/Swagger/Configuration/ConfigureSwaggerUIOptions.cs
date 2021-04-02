using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyWarehouse.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace MyWarehouse.WebApi.Swagger.Configuration
{
    internal class ConfigureSwaggerUIOptions : IConfigureOptions<SwaggerUIOptions>
    {
        private readonly IConfiguration _configuration;

        public ConfigureSwaggerUIOptions(IConfiguration configuration) 
            => _configuration = configuration;

        public void Configure(SwaggerUIOptions options)
        {
            var swaggerSettings = _configuration.GetMyOptions<SwaggerSettings>();

            options.SwaggerEndpoint(
                url: "/swagger/v1/swagger.json",
                name: swaggerSettings.ApiName + "v1"
            );
        }
    }
}