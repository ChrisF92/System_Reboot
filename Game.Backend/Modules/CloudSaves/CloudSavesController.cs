using Microsoft.AspNetCore.Mvc;

namespace Game.Backend.Modules.CloudSaves;

[ApiController]
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
        return await _cloudSaveService.GetLatestAsync(playerId, cancellationToken);
    }

    [HttpPut("{playerId:guid}")]
    public async Task<ActionResult<CloudSaveResponse>> Upsert(
        Guid playerId,
        UpsertCloudSaveRequest? request,
        CancellationToken cancellationToken)
    {
        return await _cloudSaveService.UpsertAsync(playerId, request, cancellationToken);
    }
}
