namespace MyWarehouse.Infrastructure.Authentication.External.Model
{
    public record ExternalUserData
    {
        public string Email { get; init; }
        public bool EmailVerified { get; init; }
        public string FullName { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}
