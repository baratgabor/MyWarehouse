using MyWarehouse.Infrastructure.Authentication.Core.Model;
using System.Threading.Tasks;

namespace MyWarehouse.Infrastructure.Authentication.Core.Services
{
    public interface IUserService
    {
        Task<(MySignInResult result, SignInData data)> SignIn(string username, string password);
    }
}
