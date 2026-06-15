using Microsoft.AspNetCore.Mvc;

namespace Game.Backend.Modules.GameConfig;

[ApiController]
[Route("api/v1/game-config")]
public sealed class GameConfigController : ControllerBase
{
    private readonly GameConfigService _gameConfigService;

    public GameConfigController(GameConfigService gameConfigService)
    {
        _gameConfigService = gameConfigService;
    }

    [HttpGet]
    public ActionResult<GameConfigResponse> GetCurrent()
    {
        return _gameConfigService.GetCurrentConfig();
    }
}
