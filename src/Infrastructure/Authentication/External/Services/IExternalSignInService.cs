using MyWarehouse.Infrastructure.Authentication.Core.Model;
using MyWarehouse.Infrastructure.Authentication.External.Model;
using System.Threading.Tasks;

namespace MyWarehouse.Infrastructure.Authentication.External.Services
{
    public interface IExternalSignInService
    {
        Task<(MySignInResult result, SignInData data)> SignInExternal(ExternalAuthenticationProvider provider, string idToken);
    }
}