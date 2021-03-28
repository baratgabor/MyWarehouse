using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.WebApi.ErrorHandling
{
    [ExcludeFromCodeCoverage]
    internal static class ErrorHandlingStartup
    {
        public static void AddMyErrorHandling(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(o =>
            {
                if (o == null)
                {
                    throw new ArgumentException($"Cannot find {nameof(MvcOptions)}. This module depends on MVC being already added, via e.g. AddControllers().");
                }

                o.Filters.Add<ExceptionMappingFilter>();
            });
        }
    }
}
