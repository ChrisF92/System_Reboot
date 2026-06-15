using Game.Backend.Modules.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Backend.Modules.Resources;

[ApiController]
[Authorize]
[Route("api/v1/players/{playerId:guid}/resources")]
public sealed class ResourcesController : ControllerBase
{
    private readonly ResourceService _resourceService;

    public ResourcesController(ResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpGet]
    public async Task<ActionResult<PlayerResourcesResponse>> GetResources(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        return await _resourceService.GetResourcesAsync(
            CurrentAccount.GetAccountId(User),
            playerId,
            cancellationToken);
    }

    [HttpPost("claim-offline")]
    public async Task<ActionResult<OfflineResourceClaimResponse>> ClaimOffline(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        return await _resourceService.ClaimOfflineResourcesAsync(
            CurrentAccount.GetAccountId(User),
            playerId,
            cancellationToken);
    }
}
