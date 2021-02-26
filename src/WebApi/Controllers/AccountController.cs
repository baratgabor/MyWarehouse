using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWarehouse.Infrastructure.Authentication.Services;
using MyWarehouse.WebApi.Authentication.Models.Dtos;
using System.Threading.Tasks;

namespace MyWarehouse.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
            => _userService = userService;

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto login)
            => LoginInternal(login);

        [AllowAnonymous]
        [HttpPost]
        [Route("loginForm")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        public Task<ActionResult<TokenResponseDto>> LoginForm([FromForm] LoginDto login)
            => LoginInternal(login);

        private async Task<ActionResult<TokenResponseDto>> LoginInternal(LoginDto login)
        {
            var (result, token) = await _userService.SignIn(login.Username, login.Password);

            if (result.IsLockedOut)
                return Forbid("User locked out.");
            else if (result.IsNotAllowed)
                return Forbid("User is not allowed to sign in.");
            else if (!result.Succeeded)
                return Unauthorized("Username or password incorrect.");

            return Ok(new TokenResponseDto()
            {
                access_token = token.AccessToken,
                token_type = token.TokenType,
                expires_in = token.GetRemainingLifetimeSeconds(),
                username = token.Username
            });
        }
    }
}
