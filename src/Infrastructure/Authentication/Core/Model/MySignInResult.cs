namespace MyWarehouse.Infrastructure.Authentication.Core.Model;

// Used instead of Identity.SignInResult to avoid unnecessary coupling
// between authentication pipeline participants and Identity.
public enum MySignInResult
{
    Failed,
    Success,
    LockedOut,
    NotAllowed
}
