using System.Security.Claims;
using System.Text.Encodings.Web;
using Game.Backend.Shared.Errors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Game.Backend.Modules.Auth;

public sealed class BearerSessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "BearerSession";

    private readonly AuthService _authService;

    public BearerSessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        AuthService authService)
        : base(options, logger, encoder)
    {
        _authService = authService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return AuthenticateResult.NoResult();
        }

        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Authorization header must use the Bearer scheme.");
        }

        var accessToken = authorizationHeader["Bearer ".Length..].Trim();
        var accountId = await _authService.GetAccountIdForTokenAsync(accessToken, Context.RequestAborted);
        if (accountId is null)
        {
            return AuthenticateResult.Fail("Bearer token is invalid or expired.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, accountId.Value.ToString()),
            new Claim("account_id", accountId.Value.ToString())
        };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        await Response.WriteAsJsonAsync(
            new ErrorEnvelope(new ErrorDetails("authentication_required", "Authentication is required.")));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        await Response.WriteAsJsonAsync(
            new ErrorEnvelope(new ErrorDetails("forbidden", "You do not have access to this resource.")));
    }
}
