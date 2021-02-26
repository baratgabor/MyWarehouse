using Microsoft.AspNetCore.Http;
using Serilog.Events;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.WebApi.Logging.Helper
{
    [ExcludeFromCodeCoverage]
    internal static class LogHelper
    {
        /// <summary>
        /// Replacement of the default implementation of RequestLoggingOptions.GetLevel.
        /// Excludes health check endpoints from logging by decreasing their log level.
        /// </summary>
        public static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception ex) =>
            ex != null
                ? LogEventLevel.Error
                : ctx.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                        ? LogEventLevel.Verbose // Was a health check, use Verbose
                        : LogEventLevel.Information;

        private static bool IsHealthCheckEndpoint(HttpContext ctx)
        {
            var endpoint = ctx.GetEndpoint();
            if (endpoint is object)
            {
                return string.Equals(
                    endpoint.DisplayName,
                    "Health checks",
                    StringComparison.Ordinal);
            }
            // No endpoint, so not a health check endpoint
            return false;
        }
    }
}