using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MyWarehouse.WebApi.ErrorHandling
{
    public static class ErrorHandlingStartup
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
