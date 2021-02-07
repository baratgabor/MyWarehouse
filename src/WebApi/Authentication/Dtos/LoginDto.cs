using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.WebApi.Authentication.Models.Dtos
{
    public class LoginDto
    {
        [Required, MinLength(1)]
        public string Username { get; set; }

        [Required, MinLength(1)]
        public string Password { get; set; }
    }
}
