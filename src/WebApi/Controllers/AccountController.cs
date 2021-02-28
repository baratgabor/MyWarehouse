using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWarehouse.Infrastructure.Authentication.Core.Model;
using MyWarehouse.Infrastructure.Authentication.Core.Services;
using MyWarehouse.Infrastructure.Authentication.External.Services;
using MyWarehouse.WebApi.Authentication.Models.Dtos;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MyWarehouse.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IExternalSignInService _externalSignInService;

        public AccountController(IUserService userService, IExternalSignInService externalSignInService)
        {
            _userService = userService;
            _externalSignInService = externalSignInService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto login)
            => ProduceLoginResponse(
                await _userService.SignIn(login.Username, login.Password));

        [AllowAnonymous]
        [HttpPost("loginForm")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<LoginResponseDto>> LoginForm([FromForm] LoginDto login)
            => ProduceLoginResponse(
                await _userService.SignIn(login.Username, login.Password));

        [AllowAnonymous]
        [HttpPost("loginExternal")]
        public async Task<ActionResult<LoginResponseDto>> ExternalLogin(ExternalLoginDto login)
            => ProduceLoginResponse(
                await _externalSignInService.SignInExternal(login.Provider, login.IdToken));

        private ActionResult<LoginResponseDto> ProduceLoginResponse((MySignInResult result, SignInData data) loginResults)
        {
            var (result, data) = loginResults;

            return result switch
            { 
                MySignInResult.Failed => Unauthorized("Username or password incorrect."),
                MySignInResult.LockedOut => Forbid("User is temporarily locked out."),
                MySignInResult.NotAllowed => Forbid("User is not allowed to sign in."),
                MySignInResult.Success => Ok(new LoginResponseDto()
                {
                    AccessToken = data.Token.AccessToken,
                    TokenType = data.Token.TokenType,
                    ExpiresIn = data.Token.GetRemainingLifetimeSeconds(),
                    Username = data.Username,
                    Email = data.Email,
                    IsExternalLogin = data.IsExternalLogin,
                    ExternalAuthenticationProvider = data.ExternalAuthenticationProvider
                }),
                _ => throw new InvalidEnumArgumentException($"Unknown sign-in result '{result}'.")
            };
        }
    }
}
