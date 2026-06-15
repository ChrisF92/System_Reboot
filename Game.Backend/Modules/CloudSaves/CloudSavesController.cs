using Game.Backend.Modules.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Backend.Modules.CloudSaves;

[ApiController]
[Authorize]
[Route("api/v1/cloud-saves")]
public sealed class CloudSavesController : ControllerBase
{
    private readonly CloudSaveService _cloudSaveService;

    public CloudSavesController(CloudSaveService cloudSaveService)
    {
        _cloudSaveService = cloudSaveService;
    }

    [HttpGet("{playerId:guid}")]
    public async Task<ActionResult<CloudSaveResponse>> GetLatest(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.GetAccountId(User);
        return await _cloudSaveService.GetLatestAsync(accountId, playerId, cancellationToken);
    }

    [HttpPut("{playerId:guid}")]
    public async Task<ActionResult<CloudSaveResponse>> Upsert(
        Guid playerId,
        UpsertCloudSaveRequest? request,
        CancellationToken cancellationToken)
    {
        var accountId = CurrentAccount.GetAccountId(User);
        return await _cloudSaveService.UpsertAsync(accountId, playerId, request, cancellationToken);
    }
}
