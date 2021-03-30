using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MyWarehouse.Infrastructure;
using MyWarehouse.WebApi.Swagger.Configuration;
using MyWarehouse.WebApi.Swagger.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.WebApi.Swagger
{
    [ExcludeFromCodeCoverage]
    internal static class SwaggerStartup
    {
        public static void AddMySwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                var swaggerSettings = configuration.GetMyOptions<SwaggerSettings>();

                if (swaggerSettings.UseSwagger == false)
                {
                    return;
                }

                c.SwaggerDoc(swaggerSettings.ApiVersion, new OpenApiInfo { Title = swaggerSettings.ApiName, Version = swaggerSettings.ApiVersion });

                // Add Login capability to Swagger UI.
                c.AddSecurityDefinition(SecuritySchemeNames.ApiLogin, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow()
                        {
                            TokenUrl = new Uri(swaggerSettings.LoginPath, UriKind.Relative)
                        }
                    }
                });

                // Prevent SwaggerGen from throwing exception when multiple DTOs from different namespaces have the same type name.
                c.CustomSchemaIds(x =>
                {
                    var lastNamespaceSection = x.Namespace[(x.Namespace.LastIndexOf('.') + 1)..];
                    var genericParameters = string.Join(',', (IEnumerable<Type>)x.GetGenericArguments());

                    return $"{lastNamespaceSection}.{x.Name}{(string.IsNullOrEmpty(genericParameters) ? null : "<" + genericParameters + ">")}";
                });

                c.OperationFilter<SwaggerGroupFilter>();
                c.OperationFilter<SwaggerAuthorizeFilter>();
            });
        }

        public static void UseMySwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            var swaggerSettings = configuration.GetMyOptions<SwaggerSettings>();

            if (swaggerSettings.UseSwagger == true)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(
                        url: swaggerSettings.JsonEndpointPath,
                        name: swaggerSettings.ApiName + swaggerSettings.ApiVersion
                    );
                });
            }
        }
    }
}
