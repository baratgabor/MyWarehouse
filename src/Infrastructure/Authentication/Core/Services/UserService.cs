using Microsoft.AspNetCore.Identity;
using MyWarehouse.Infrastructure.Authentication.Core.Model;
using MyWarehouse.Infrastructure.Identity.Model;

namespace MyWarehouse.Infrastructure.Authentication.Core.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<(MySignInResult result, SignInData? data)> SignIn(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return (MySignInResult.Failed, null);
        }

        // Don't use SignInManager.PasswordSignInAsync(), because that sets useless cookies.
        // But 'CheckPasswordSignInAsync' doesn't. Yep, it's confusing. Good thing we have access to the source code. :D
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return (MySignInResult.LockedOut, null);
            if (result.IsNotAllowed)
                return (MySignInResult.NotAllowed, null);
            throw new System.Exception("Unhandled sign-in outcome.");
        }

        var token = _tokenService.CreateAuthenticationToken(user.Id, username);

        return (
            MySignInResult.Success,
            data: new SignInData()
            {
                Username = user.UserName,
                Email = user.Email,
                Token = token
            }
        );
    }
}
