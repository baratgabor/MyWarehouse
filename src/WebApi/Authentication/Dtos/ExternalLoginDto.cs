using MyWarehouse.Infrastructure.Authentication.External.Model;
using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.WebApi.Authentication.Models.Dtos
{
    public record ExternalLoginDto
    {
        public ExternalAuthenticationProvider Provider { get; init; }

        [Required, MinLength(1)]
        public string IdToken { get; init; }
    }
}
