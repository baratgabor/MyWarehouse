using MyWarehouse.Infrastructure.Authentication.External.Model;

namespace MyWarehouse.WebApi.Authentication.Models.Dtos;

public record ExternalLoginDto
{
    public ExternalAuthenticationProvider Provider { get; init; }

    [Required, MinLength(1)]
    public string IdToken { get; init; } = null!;
}
