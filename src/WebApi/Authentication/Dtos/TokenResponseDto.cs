namespace MyWarehouse.WebApi.Authentication.Models.Dtos
{
    /// <summary>
    /// Standard token response for login.
    /// </summary>
    public class TokenResponseDto
    {
        /// <summary>
        /// The generated access token.
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// The stored refresh token.
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// The type of the token. Usually "Bearer".
        /// </summary>
        public string token_type { get; set; } = "Bearer";

        /// <summary>
        /// The expiration of the token, set in minutes.
        /// </summary>
        public int expires_in { get; set; }

        public string username { get; set; }
    }
}
