using Microsoft.AspNetCore.Identity;
using MyWarehouse.Infrastructure.Authentication.Model;
using System.Threading.Tasks;

namespace MyWarehouse.Infrastructure.Authentication.Services
{
    public interface IUserService
    {
        Task<(SignInResult result, TokenModel token)> SignIn(string username, string password);
    }
}
