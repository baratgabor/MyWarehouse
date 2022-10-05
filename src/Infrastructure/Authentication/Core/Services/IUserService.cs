using MyWarehouse.Infrastructure.Authentication.Core.Model;

namespace MyWarehouse.Infrastructure.Authentication.Core.Services;

public interface IUserService
{
    Task<(MySignInResult result, SignInData? data)> SignIn(string username, string password);
}
