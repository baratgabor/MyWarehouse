using System;

namespace MyWarehouse.Infrastructure.Authentication.Model
{
    public class TokenModel
    {
        public string TokenType { get; }
        public string AccessToken { get; }
        public DateTime ExpiresAt {get;}
        public string Username { get; set; }

        public TokenModel(string tokenType, string accessToken, DateTime expiresAt)
        {
            TokenType = tokenType;
            AccessToken = accessToken;
            ExpiresAt = expiresAt;
        }

        public int GetRemainingLifetimeSeconds()
            => Math.Max(0, (int)(ExpiresAt - DateTime.Now).TotalSeconds);
    }
}
