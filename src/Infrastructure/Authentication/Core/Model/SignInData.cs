namespace MyWarehouse.Infrastructure.Authentication.Core.Model
{
    public record SignInData
    {
        public TokenModel Token { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public bool IsExternalLogin => !string.IsNullOrWhiteSpace(ExternalAuthenticationProvider);
        public string ExternalAuthenticationProvider { get; init; }
    }
}
