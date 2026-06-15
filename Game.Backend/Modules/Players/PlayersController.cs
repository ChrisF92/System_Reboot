using Game.Backend.Modules.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Backend.Modules.Players;

[ApiController]
[Authorize]
[Route("api/v1/players")]
public sealed class PlayersController : ControllerBase
{
    private readonly PlayerService _playerService;

    public PlayersController(PlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpPost]
    public async Task<ActionResult<PlayerProfileResponse>> CreatePlayer(
        CreatePlayerRequest? request,
        CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.GetAccountId(User);
        var player = await _playerService.CreatePlayerAsync(accountId, request, cancellationToken);
        return CreatedAtAction(nameof(GetPlayer), new { playerId = player.PlayerId }, player);
    }

    [HttpGet("{playerId:guid}")]
    public async Task<ActionResult<PlayerProfileResponse>> GetPlayer(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.GetAccountId(User);
        return await _playerService.GetPlayerAsync(accountId, playerId, cancellationToken);
    }
}
