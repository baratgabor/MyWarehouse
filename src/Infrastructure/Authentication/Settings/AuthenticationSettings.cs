using System;
using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.Infrastructure.Authentication.Settings
{
    public class AuthenticationSettings
    {
        [Required, MinLength(10)]
        public string JwtIssuer { get; init; }

        [Required, MinLength(1)]
        public string JwtAudience { get; init; }

        [Required, MinLength(10)]
        public string JwtSigningKeyBase64
        {
            get => _jwtSigningKeyBase64;
            init { _jwtSigningKeyBase64 = value; JwtSigningKey = Convert.FromBase64String(value); }
        }
        private string _jwtSigningKeyBase64;

        public byte[] JwtSigningKey { get; private set; }

        [Range(60, int.MaxValue)]
        public int TokenExpirationSeconds { get; init; }
    }
}
