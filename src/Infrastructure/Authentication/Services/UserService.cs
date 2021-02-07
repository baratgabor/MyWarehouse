using Microsoft.AspNetCore.Identity;
using MyWarehouse.Infrastructure.Authentication.Model;
using MyWarehouse.Infrastructure.Identity.Model;
using System.Threading.Tasks;

namespace MyWarehouse.Infrastructure.Authentication.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._tokenService = tokenService;
        }

        public async Task<(SignInResult result, TokenModel token)> SignIn(string username, string password)
        {
            //var user = await _userManager.FindByEmailAsync(username);
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return (SignInResult.Failed, null);

            // Don't use SignInManager.PasswordSignInAsync(), because that sets useless cookies.
            // But 'CheckPasswordSignInAsync' doesn't. Yep, it's confusing. Good thing we have access to the source code. :D
            var result =  await _signInManager.CheckPasswordSignInAsync(user, password, true);

            TokenModel token = null;
            if (result.Succeeded)
            {
                token = _tokenService.CreateAuthenticationToken(user.Id, user.UserName);
                token.Username = user.UserName;
            }

            return (result, token);
        }
    }
}
