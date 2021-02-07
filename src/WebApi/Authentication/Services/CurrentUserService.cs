using Microsoft.AspNetCore.Http;
using MyWarehouse.Application.Dependencies.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyWarehouse.WebApi.Authentication.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private const string DefaultNonUserMoniker = "System";

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null)
            {
                UserId = DefaultNonUserMoniker;
            }
            else
            {
                UserId = httpContextAccessor.HttpContext.User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
            }
        }

        public string UserId { get; }
    }
}
