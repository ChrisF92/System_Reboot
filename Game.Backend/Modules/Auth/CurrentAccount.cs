using System.Security.Claims;

namespace Game.Backend.Modules.Auth;

public static class CurrentAccount
{
    public static Guid GetAccountId(ClaimsPrincipal user)
    {
        var accountIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(accountIdValue, out var accountId))
        {
            throw new InvalidOperationException("Authenticated request is missing an account id claim.");
        }

        return accountId;
    }
}
