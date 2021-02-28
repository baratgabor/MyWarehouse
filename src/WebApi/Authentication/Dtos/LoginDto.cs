using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.WebApi.Authentication.Models.Dtos
{
    public record LoginDto
    {
        [Required, MinLength(1)]
        public string Username { get; init; }

        [Required, MinLength(1)]
        public string Password { get; init; }
    }
}
