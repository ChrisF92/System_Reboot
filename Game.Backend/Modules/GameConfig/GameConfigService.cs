using System.Text.Json;
using Game.Backend.Shared.Errors;

namespace Game.Backend.Modules.GameConfig;

public sealed class GameConfigService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly GameConfigRepository _gameConfigRepository;
    private readonly TimeProvider _timeProvider;

    public GameConfigService(GameConfigRepository gameConfigRepository, TimeProvider timeProvider)
    {
        _gameConfigRepository = gameConfigRepository;
        _timeProvider = timeProvider;
    }

    public async Task<GameConfigResponse> GetCurrentConfigAsync(CancellationToken cancellationToken)
    {
        var config = await _gameConfigRepository.GetActiveAsync(cancellationToken);
        if (config is null)
        {
            throw new NotFoundException("game_config_not_found", "Active game config was not found.");
        }

        return Deserialize(config.ConfigJson);
    }

    public async Task<GameConfigResponse> GetConfigVersionAsync(
        string configVersion,
        CancellationToken cancellationToken)
    {
        var config = await _gameConfigRepository.GetByVersionAsync(configVersion, cancellationToken);
        if (config is null)
        {
            throw new NotFoundException("game_config_not_found", "Game config version was not found.");
        }

        return Deserialize(config.ConfigJson);
    }

    public async Task ActivateConfigVersionAsync(
        string configVersion,
        CancellationToken cancellationToken)
    {
        _ = await GetConfigVersionAsync(configVersion, cancellationToken);
        await _gameConfigRepository.ActivateAsync(
            configVersion,
            _timeProvider.GetUtcNow(),
            cancellationToken);
    }

    private static GameConfigResponse Deserialize(string configJson)
    {
        var config = JsonSerializer.Deserialize<GameConfigResponse>(configJson, JsonOptions);
        if (config is null)
        {
            throw new InvalidOperationException("Stored game config could not be deserialized.");
        }

        return config;
    }
}
