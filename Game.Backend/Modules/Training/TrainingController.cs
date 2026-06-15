using Game.Backend.Modules.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Backend.Modules.Training;

[ApiController]
[Authorize]
[Route("api/v1/players/{playerId:guid}/training")]
public sealed class TrainingController : ControllerBase
{
    private readonly TrainingService _trainingService;

    public TrainingController(TrainingService trainingService)
    {
        _trainingService = trainingService;
    }

    [HttpGet]
    public async Task<ActionResult<TrainingResponse>> GetTraining(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        return await _trainingService.GetTrainingAsync(
            CurrentAccount.GetAccountId(User),
            playerId,
            cancellationToken);
    }

    [HttpPost("{statId}/upgrade")]
    public async Task<ActionResult<UpgradeTrainingResponse>> Upgrade(
        Guid playerId,
        string statId,
        UpgradeTrainingRequest? request,
        CancellationToken cancellationToken)
    {
        return await _trainingService.UpgradeTrainingAsync(
            CurrentAccount.GetAccountId(User),
            playerId,
            statId,
            request,
            cancellationToken);
    }
}
