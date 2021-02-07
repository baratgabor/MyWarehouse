using System;

namespace MyWarehouse.Infrastructure.Swagger.Filters
{
    /// <summary>
    /// Specifies a custom name that overrides Swagger's default group name for the actions in the given controller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SwaggerGroupAttribute : Attribute
    {
        public string GroupName { get; set; }

        public SwaggerGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
