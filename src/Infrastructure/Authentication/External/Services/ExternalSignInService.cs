using MyWarehouse.Infrastructure.Authentication.Core.Model;
using MyWarehouse.Infrastructure.Authentication.Core.Services;
using MyWarehouse.Infrastructure.Authentication.External.Exceptions;
using MyWarehouse.Infrastructure.Authentication.External.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWarehouse.Infrastructure.Authentication.External.Services
{
    public class ExternalSignInService : IExternalSignInService
    {
        private readonly IExternalAuthenticationVerifier _verifier;
        private readonly ITokenService _tokenService;

        public ExternalSignInService(IExternalAuthenticationVerifier verifier, ITokenService tokenService)
        {
            _verifier = verifier;
            _tokenService = tokenService;
        }

        public async Task<(MySignInResult result, SignInData data)> SignInExternal(ExternalAuthenticationProvider provider, string idToken)
        {
            var (success, userData) = await _verifier.Verify(provider, idToken);

            if (!success)
            {
                return (MySignInResult.Failed, null);
            }

            if (string.IsNullOrWhiteSpace(userData.Email) || string.IsNullOrWhiteSpace(userData.FirstName))
            {
                var missingFields = new List<string>();
                if (string.IsNullOrWhiteSpace(userData.Email)) missingFields.Add(nameof(ExternalUserData.Email));
                if (string.IsNullOrWhiteSpace(userData.FirstName)) missingFields.Add(nameof(ExternalUserData.FirstName));

                throw new ExternalAuthenticationInfoException(
                    missingFields: missingFields,
                    receivedData: userData
                );
            }

            // Currently, in this sample app, we're not creating new local users for externally authenticated users.
            // Partically because that might involve privacy concerns.
            // TODO: Consider if user creation is needed, or if external logins should be registered in Identity tables.

            var token = _tokenService.CreateAuthenticationToken($"ext:{userData.Email}", userData.Email, new[] { ("ExternalAuthority", provider.ToString()) });

            return (
                result: MySignInResult.Success,
                data: new SignInData()
                {
                    ExternalAuthenticationProvider = provider.ToString(),
                    Username = userData.FullName,
                    Email = userData.Email,
                    Token = token
                }
            );
        }
    }
}
