using Game.Backend.Shared.Errors;

namespace Game.Backend.Modules.Players;

public sealed class PlayerService
{
    private const int MinimumDisplayNameLength = 3;
    private const int MaximumDisplayNameLength = 24;

    private readonly PlayerRepository _playerRepository;
    private readonly TimeProvider _timeProvider;

    public PlayerService(PlayerRepository playerRepository, TimeProvider timeProvider)
    {
        _playerRepository = playerRepository;
        _timeProvider = timeProvider;
    }

    public async Task<PlayerProfileResponse> CreatePlayerAsync(
        Guid accountId,
        CreatePlayerRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ValidationException("missing_player_request", "Player creation request is required.");
        }

        var displayName = ValidateDisplayName(request.DisplayName);
        var now = _timeProvider.GetUtcNow();
        var player = new Player(
            Guid.NewGuid(),
            accountId,
            displayName,
            Level: 1,
            Xp: 0,
            Resources: new ResourceWallet(0, 0, 0, 0, 0),
            CreatedAtUtc: now,
            LastResourceClaimedAtUtc: now);

        await _playerRepository.AddAsync(player, cancellationToken);
        return ToResponse(player);
    }

    public async Task<PlayerProfileResponse> GetPlayerAsync(
        Guid accountId,
        Guid playerId,
        CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player is null)
        {
            throw new NotFoundException("player_not_found", "Player profile was not found.");
        }

        if (player.AccountId != accountId)
        {
            throw new ForbiddenException("player_forbidden", "Player profile belongs to another account.");
        }

        return ToResponse(player);
    }

    public async Task EnsurePlayerOwnedByAccountAsync(
        Guid accountId,
        Guid playerId,
        CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player is null)
        {
            throw new NotFoundException("player_not_found", "Player profile was not found.");
        }

        if (player.AccountId != accountId)
        {
            throw new ForbiddenException("player_forbidden", "Player profile belongs to another account.");
        }
    }

    private static string ValidateDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ValidationException("display_name_required", "Display name is required.");
        }

        var trimmedName = displayName.Trim();
        if (trimmedName.Length < MinimumDisplayNameLength)
        {
            throw new ValidationException(
                "display_name_too_short",
                $"Display name must be at least {MinimumDisplayNameLength} characters.");
        }

        if (trimmedName.Length > MaximumDisplayNameLength)
        {
            throw new ValidationException(
                "display_name_too_long",
                $"Display name must be no more than {MaximumDisplayNameLength} characters.");
        }

        return trimmedName;
    }

    private static PlayerProfileResponse ToResponse(Player player)
    {
        return new PlayerProfileResponse(
            player.PlayerId,
            player.DisplayName,
            player.Level,
            player.Xp,
            new ResourceWalletResponse(
                player.Resources.Matter,
                player.Resources.Energy,
                player.Resources.Data,
                player.Resources.CoreFragments,
                player.Resources.QuantumCores),
            player.CreatedAtUtc);
    }
}
