using MyWarehouse.Infrastructure.Authentication.External.Model;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.Infrastructure.Authentication.External.Services;

public interface IExternalAuthenticationVerifier
{
    Task<(bool success, ExternalUserData? userData)> Verify(ExternalAuthenticationProvider provider, string idToken);
}
