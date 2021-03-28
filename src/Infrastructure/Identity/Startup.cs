using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWarehouse.Infrastructure.Identity.Model;
using MyWarehouse.Infrastructure.Persistence.Context;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;

namespace MyWarehouse.Infrastructure.Identity
{
    [ExcludeFromCodeCoverage]
    internal static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration _)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub; // JWT specific
            })
                .AddDefaultTokenProviders()

                // Adding Roles is optional, and mostly exists for backwards-compatibility.
                // Not needed if policy/claim based authorization is used (which is recommended).
                // But, if AddRoles() is called, it must be before calling AddEntityFrameworkStores(), because otherwise IRoleStore won't be added (despite what the summary says)..
                //.AddRoles<IdentityRole>()

                .AddEntityFrameworkStores<ApplicationDbContext>(); // EF specific
        }
    }
}
