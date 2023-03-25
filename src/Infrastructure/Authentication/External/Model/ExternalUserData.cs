namespace MyWarehouse.Infrastructure.Authentication.External.Model;

public record ExternalUserData
{
    public string Email { get; init; } = null!;
    public bool EmailVerified { get; init; }
    public string FullName { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
}
