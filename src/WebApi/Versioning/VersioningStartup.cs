using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using MyWarehouse.WebApi.Versioning.SwaggerConfiguration;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace MyWarehouse.WebApi.Versioning;

// This module is only semantically meaningful; versioning can't be fully modularized.
// If the AddApiVersioning() extension method here isn't called, the versioned Route attribute
// on controllers will throw on exception.
[ExcludeFromCodeCoverage]
internal static class VersioningStartup
{
    /// <summary>
    /// Adds versioning support to the API.
    /// If it's desired to add versioning support for Swagger too,
    /// this method must be called after AddSwagger().
    /// </summary>
    public static void AddMyVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
        });

        // If Swagger is enabled, add versioning support to it.
        if (services.Any(x => x.ServiceType == typeof(ISwaggerProvider)))
        {
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AddApiVersionParametersWhenVersionNeutral = true;
            });

            // Override Swagger configurations with versioned ones.
            services.AddTransient<IPostConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
            services.AddTransient<IPostConfigureOptions<SwaggerUIOptions>, ConfigureSwaggerUIOptions>();
        }
    }
}
