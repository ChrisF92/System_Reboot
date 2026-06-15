using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Backend.Modules.Auth;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterAccountRequest? request,
        CancellationToken cancellationToken)
    {
        return await _authService.RegisterAsync(request, cancellationToken);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest? request,
        CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(request, cancellationToken);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<LogoutResponse>> Logout(CancellationToken cancellationToken)
    {
        return await _authService.LogoutAsync(
            CurrentAccount.GetAccountId(User),
            CurrentAccount.GetSessionId(User),
            cancellationToken);
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken cancellationToken)
    {
        return await _authService.RefreshAsync(
            CurrentAccount.GetAccountId(User),
            CurrentAccount.GetSessionId(User),
            cancellationToken);
    }
}
