using System.Text.Json;
using Game.Backend.Modules.Players;
using Game.Backend.Shared.Errors;

namespace Game.Backend.Modules.CloudSaves;

public sealed class CloudSaveService
{
    private const int MaximumSaveJsonLength = 131_072;
    private const int MaximumChecksumLength = 128;

    private readonly CloudSaveRepository _cloudSaveRepository;
    private readonly PlayerService _playerService;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<CloudSaveService> _logger;

    public CloudSaveService(
        CloudSaveRepository cloudSaveRepository,
        PlayerService playerService,
        TimeProvider timeProvider,
        ILogger<CloudSaveService> logger)
    {
        _cloudSaveRepository = cloudSaveRepository;
        _playerService = playerService;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<CloudSaveResponse> GetLatestAsync(
        Guid playerId,
        CancellationToken cancellationToken)
    {
        await _playerService.EnsurePlayerExistsAsync(playerId, cancellationToken);
        var save = await _cloudSaveRepository.GetLatestAsync(playerId, cancellationToken);
        if (save is null)
        {
            throw new NotFoundException("cloud_save_not_found", "Cloud save was not found.");
        }

        return ToResponse(save);
    }

    public async Task<CloudSaveResponse> UpsertAsync(
        Guid playerId,
        UpsertCloudSaveRequest? request,
        CancellationToken cancellationToken)
    {
        await _playerService.EnsurePlayerExistsAsync(playerId, cancellationToken);
        if (request is null)
        {
            throw new ValidationException("missing_cloud_save_request", "Cloud save request is required.");
        }

        ValidateRequest(request);

        var existingSave = await _cloudSaveRepository.GetLatestAsync(playerId, cancellationToken);
        if (existingSave is not null && request.SaveVersion < existingSave.SaveVersion)
        {
            throw new ValidationException(
                "save_version_regression",
                "Cloud save version cannot go backwards.");
        }

        var save = new CloudSave(
            playerId,
            request.SaveVersion,
            request.SaveJson!,
            request.Checksum,
            request.ClientSavedAtUtc,
            _timeProvider.GetUtcNow());

        var storedSave = await _cloudSaveRepository.UpsertAsync(save, cancellationToken);
        _logger.LogInformation(
            "Stored cloud save for player {PlayerId} at save version {SaveVersion}.",
            playerId,
            request.SaveVersion);

        return ToResponse(storedSave);
    }

    private static void ValidateRequest(UpsertCloudSaveRequest request)
    {
        if (request.SaveVersion < 1)
        {
            throw new ValidationException("invalid_save_version", "Save version must be at least 1.");
        }

        if (string.IsNullOrWhiteSpace(request.SaveJson))
        {
            throw new ValidationException("empty_save_json", "Save JSON is required.");
        }

        if (request.SaveJson.Length > MaximumSaveJsonLength)
        {
            throw new ValidationException(
                "save_json_too_large",
                $"Save JSON must be no more than {MaximumSaveJsonLength} characters.");
        }

        if (request.Checksum is { Length: > MaximumChecksumLength })
        {
            throw new ValidationException(
                "checksum_too_long",
                $"Checksum must be no more than {MaximumChecksumLength} characters.");
        }

        try
        {
            using var _ = JsonDocument.Parse(request.SaveJson);
        }
        catch (JsonException)
        {
            throw new ValidationException("invalid_save_json", "Save JSON must be valid JSON.");
        }
    }

    private static CloudSaveResponse ToResponse(CloudSave save)
    {
        return new CloudSaveResponse(
            save.PlayerId,
            save.SaveVersion,
            save.SaveJson,
            save.Checksum,
            save.ClientSavedAtUtc,
            save.SavedAtUtc);
    }
}
