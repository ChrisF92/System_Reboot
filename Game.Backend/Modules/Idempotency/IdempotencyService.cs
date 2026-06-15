using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Game.Backend.Shared.Errors;

namespace Game.Backend.Modules.Idempotency;

public sealed class IdempotencyService
{
    private const int MaximumRequestIdLength = 96;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IdempotencyRepository _idempotencyRepository;
    private readonly TimeProvider _timeProvider;

    public IdempotencyService(
        IdempotencyRepository idempotencyRepository,
        TimeProvider timeProvider)
    {
        _idempotencyRepository = idempotencyRepository;
        _timeProvider = timeProvider;
    }

    public string ValidateRequestId(string? requestId)
    {
        if (string.IsNullOrWhiteSpace(requestId))
        {
            throw new ValidationException("request_id_required", "Request id is required.");
        }

        var trimmed = requestId.Trim();
        if (trimmed.Length > MaximumRequestIdLength)
        {
            throw new ValidationException(
                "request_id_too_long",
                $"Request id must be no more than {MaximumRequestIdLength} characters.");
        }

        return trimmed;
    }

    public string HashPayload(object payload)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash);
    }

    public async Task<TResponse?> GetReplayAsync<TResponse>(
        Guid accountId,
        string action,
        string requestId,
        string requestHash,
        CancellationToken cancellationToken)
    {
        var existingKey = await _idempotencyRepository.GetAsync(
            accountId,
            action,
            requestId,
            cancellationToken);

        if (existingKey is null)
        {
            return default;
        }

        if (!string.Equals(existingKey.RequestHash, requestHash, StringComparison.Ordinal))
        {
            throw new ConflictException(
                "idempotency_key_conflict",
                "Request id has already been used with a different payload.");
        }

        var response = JsonSerializer.Deserialize<TResponse>(existingKey.ResponseJson, JsonOptions);
        if (response is null)
        {
            throw new InvalidOperationException("Stored idempotency response could not be deserialized.");
        }

        return response;
    }

    public async Task StoreResponseAsync<TResponse>(
        Guid accountId,
        Guid? playerId,
        string action,
        string requestId,
        string requestHash,
        TResponse response,
        CancellationToken cancellationToken)
    {
        var key = new IdempotencyKey(
            Guid.NewGuid(),
            accountId,
            playerId,
            action,
            requestId,
            requestHash,
            JsonSerializer.Serialize(response, JsonOptions),
            _timeProvider.GetUtcNow());

        await _idempotencyRepository.AddAsync(key, cancellationToken);
    }
}
