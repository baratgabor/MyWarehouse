using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace MyWarehouse.Infrastructure.Swagger.Filters
{
    /// <summary>
    /// Overrides the grouping name of actions in controllers which are decorated with <see cref="SwaggerGroupAttribute"/>.
    /// </summary>
    public class SwaggerGroupFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var customGroupAttribute = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .OfType<SwaggerGroupAttribute>()?.FirstOrDefault();

            if (customGroupAttribute != null && !string.IsNullOrWhiteSpace(customGroupAttribute.GroupName))
            {
                operation.Tags = new List<OpenApiTag> { new OpenApiTag() { Name = customGroupAttribute.GroupName } };
            }
        }
    }
}
