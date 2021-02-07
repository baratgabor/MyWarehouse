using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.Infrastructure.Swagger.Configuration
{
    class SwaggerSettings
    {
        [Required, MinLength(1)]
        public string ApiName { get; init; }

        [Required, MinLength(1)]
        public string ApiVersion { get; init; }

        public bool UseSwagger { get; init; }

        [Required, MinLength(1)]
        public string LoginPath { get; set; }

        [Required, MinLength(1)]
        public string JsonEndpointPath { get; set; }
    }
}
