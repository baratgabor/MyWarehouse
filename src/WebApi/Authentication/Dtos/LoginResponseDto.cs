namespace MyWarehouse.WebApi.Authentication.Models.Dtos;

/// <summary>
/// Standard token response for login.
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// The generated access token.
    /// </summary>
    public string AccessToken { get; init; } = null!;

    /// <summary>
    /// The stored refresh token.
    /// </summary>
    // TODO: Consider supporting refresh tokens.
    //public string refresh_token { get; set; }

    /// <summary>
    /// The type of the token. Usually "Bearer".
    /// </summary>
    public string TokenType { get; init; } = "Bearer";

    /// <summary>
    /// The expiration of the token, set in minutes.
    /// </summary>
    public int ExpiresIn { get; init; }

    public string Username { get; init; } = null!;

    public string Email { get; init; } = null!;

    public string? ExternalAuthenticationProvider { get; init; }

    public bool IsExternalLogin { get; init; }
}
