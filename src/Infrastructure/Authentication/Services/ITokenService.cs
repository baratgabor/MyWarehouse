using MyWarehouse.Infrastructure.Authentication.Model;

namespace MyWarehouse.Infrastructure.Authentication.Services
{
    public interface ITokenService
    {
        TokenModel CreateAuthenticationToken(string userId, string userName);
    }
}
