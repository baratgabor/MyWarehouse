using MyWarehouse.Infrastructure.Authentication.Core.Model;

namespace MyWarehouse.Infrastructure.Authentication.Core.Services;

public interface ITokenService
{
    TokenModel CreateAuthenticationToken(string userId, string uniqueName, IEnumerable<(string claimType, string claimValue)>? customClaims = null);
}
