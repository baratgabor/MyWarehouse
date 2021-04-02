using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyWarehouse.WebApi.Versioning.SwaggerConfiguration
{
    public class ConfigureSwaggerGenOptions : IPostConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider _versionProvider;

        public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider versionProvider)
            => _versionProvider = versionProvider;

        public void PostConfigure(string _, SwaggerGenOptions options)
        {
            // Clear potentially added unversioned docs.
            options.SwaggerGeneratorOptions.SwaggerDocs.Clear();

            foreach (var description in _versionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                  description.GroupName,
                    new OpenApiInfo()
                    {
                        Title = $"{nameof(MyWarehouse)} {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                    });
            }
        }
    }
}
