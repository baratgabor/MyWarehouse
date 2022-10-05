namespace MyWarehouse.Infrastructure.Authentication.Core.Model;

public record SignInData
{
    public TokenModel Token { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public bool IsExternalLogin => !string.IsNullOrWhiteSpace(ExternalAuthenticationProvider);
    public string? ExternalAuthenticationProvider { get; init; }
}
