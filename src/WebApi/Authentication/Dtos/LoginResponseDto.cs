namespace MyWarehouse.WebApi.Authentication.Models.Dtos
{
    /// <summary>
    /// Standard token response for login.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// The generated access token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The stored refresh token.
        /// </summary>
        // TODO: Consider supporting refresh tokens.
        //public string refresh_token { get; set; }

        /// <summary>
        /// The type of the token. Usually "Bearer".
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// The expiration of the token, set in minutes.
        /// </summary>
        public int ExpiresIn { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string ExternalAuthenticationProvider { get; set; }

        public bool IsExternalLogin { get; set; }
    }
}
