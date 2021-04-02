using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.WebApi.Swagger.Configuration
{
    class SwaggerSettings
    {
        [Required, MinLength(1)]
        public string ApiName { get; init; }

        public bool UseSwagger { get; init; }

        [Required, MinLength(1)]
        public string LoginPath { get; set; }
    }
}
