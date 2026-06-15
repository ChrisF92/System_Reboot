using Game.Backend.Modules.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Backend.Modules.Upgrades;

[ApiController]
[Authorize]
[Route("api/v1/players/{playerId:guid}/upgrades")]
public sealed class UpgradesController : ControllerBase
{
    private readonly UpgradeService _upgradeService;

    public UpgradesController(UpgradeService upgradeService)
    {
        _upgradeService = upgradeService;
    }

    [HttpGet]
    public async Task<ActionResult<PlayerUpgradesResponse>> GetUpgrades(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        return await _upgradeService.GetUpgradesAsync(
            CurrentAccount.GetAccountId(User),
            playerId,
            cancellationToken);
    }

    [HttpPost("{upgradeId}/purchase")]
    public async Task<ActionResult<PurchaseUpgradeResponse>> Purchase(
        Guid playerId,
        string upgradeId,
        PurchaseUpgradeRequest? request,
        CancellationToken cancellationToken)
    {
        return await _upgradeService.PurchaseUpgradeAsync(
            CurrentAccount.GetAccountId(User),
            playerId,
            upgradeId,
            request,
            cancellationToken);
    }
}
