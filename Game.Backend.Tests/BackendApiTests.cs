using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Game.Backend.Database;
using Game.Backend.Modules.Auth;
using Game.Backend.Modules.CloudSaves;
using Game.Backend.Modules.GameConfig;
using Game.Backend.Modules.Players;
using Game.Backend.Modules.Purchases;
using Game.Backend.Modules.Resources;
using Microsoft.EntityFrameworkCore;

namespace Game.Backend.Tests;

public sealed class BackendApiTests : IClassFixture<TestBackendFactory>
{
    private readonly TestBackendFactory _factory;
    private readonly HttpClient _client;

    public BackendApiTests(TestBackendFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePlayer_ReturnsUnityFriendlyProfileContract()
    {
        await RegisterAndAuthorizeAsync(_client);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/players",
            new CreatePlayerRequest(" RebootNode "));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"playerId\"", json);
        Assert.Contains("\"displayName\"", json);

        var profile = JsonSerializer.Deserialize<PlayerProfileResponse>(
            json,
            JsonOptions.Default);

        Assert.NotNull(profile);
        Assert.Equal("RebootNode", profile.DisplayName);
        Assert.Equal(1, profile.Level);
        Assert.Equal(0, profile.Resources.Matter);
        Assert.NotEqual(Guid.Empty, profile.PlayerId);
    }

    [Fact]
    public async Task CloudSave_RoundTripsVersionedSaveJsonForExistingPlayer()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("SaveNode");
        var request = new UpsertCloudSaveRequest(
            SaveVersion: 1,
            SaveJson: """
                {
                  "version": 1,
                  "resources": {
                    "matter": 25,
                    "energy": 10,
                    "data": 3
                  }
                }
                """,
            Checksum: "test-checksum",
            ClientSavedAtUtc: DateTimeOffset.Parse("2026-06-15T16:00:00Z"));

        var saveResponse = await _client.PutAsJsonAsync(
            $"/api/v1/cloud-saves/{player.PlayerId}",
            request);

        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

        var latestSave = await _client.GetFromJsonAsync<CloudSaveResponse>(
            $"/api/v1/cloud-saves/{player.PlayerId}",
            JsonOptions.Default);

        Assert.NotNull(latestSave);
        Assert.Equal(player.PlayerId, latestSave.PlayerId);
        Assert.Equal(1, latestSave.SaveVersion);
        Assert.Equal("test-checksum", latestSave.Checksum);
        Assert.Contains("\"matter\": 25", latestSave.SaveJson);
        Assert.True(latestSave.SavedAtUtc > DateTimeOffset.MinValue);
    }

    [Fact]
    public async Task Resources_Get_ReturnsServerOwnedStartingBalances()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("ResourceNode");

        var resources = await _client.GetFromJsonAsync<PlayerResourcesResponse>(
            $"/api/v1/players/{player.PlayerId}/resources",
            JsonOptions.Default);

        Assert.NotNull(resources);
        Assert.Equal(player.PlayerId, resources.PlayerId);
        Assert.Equal(0, resources.Resources.Matter);
        Assert.Equal(0, resources.Resources.Energy);
        Assert.Equal(0, resources.Resources.Data);
        Assert.True(resources.LastResourceClaimedAtUtc >= player.CreatedAtUtc.AddSeconds(-1));
    }

    [Fact]
    public async Task Resources_ClaimOffline_AppliesConfiguredCapAndEfficiency()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("OfflineNode");
        await SetLastResourceClaimedAtUtcAsync(
            player.PlayerId,
            DateTimeOffset.UtcNow.AddHours(-3));

        var claim = await _client.PostAsJsonAsync(
            $"/api/v1/players/{player.PlayerId}/resources/claim-offline",
            new ClaimOfflineResourcesRequest("offline-cap-request"));

        Assert.Equal(HttpStatusCode.OK, claim.StatusCode);

        var result = await claim.Content.ReadFromJsonAsync<OfflineResourceClaimResponse>(JsonOptions.Default);
        Assert.NotNull(result);
        Assert.Equal(player.PlayerId, result.PlayerId);
        Assert.True(result.ElapsedSeconds >= 7_200);
        Assert.Equal(7_200, result.AppliedSeconds);
        Assert.Equal(3_600, result.Gains.Matter);
        Assert.Equal(2_160, result.Gains.Energy);
        Assert.Equal(1_260, result.Gains.Data);
        Assert.Equal(3_600, result.Resources.Matter);
        Assert.Equal(2_160, result.Resources.Energy);
        Assert.Equal(1_260, result.Resources.Data);
    }

    [Fact]
    public async Task Resources_ClaimOffline_RapidDuplicateReturnsZeroGain()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("RapidNode");
        await SetLastResourceClaimedAtUtcAsync(
            player.PlayerId,
            DateTimeOffset.UtcNow.AddHours(-3));

        var firstClaim = await _client.PostAsJsonAsync(
            $"/api/v1/players/{player.PlayerId}/resources/claim-offline",
            new ClaimOfflineResourcesRequest("rapid-first-request"));
        firstClaim.EnsureSuccessStatusCode();

        var secondClaim = await _client.PostAsJsonAsync(
            $"/api/v1/players/{player.PlayerId}/resources/claim-offline",
            new ClaimOfflineResourcesRequest("rapid-second-request"));
        secondClaim.EnsureSuccessStatusCode();

        var result = await secondClaim.Content.ReadFromJsonAsync<OfflineResourceClaimResponse>(JsonOptions.Default);
        Assert.NotNull(result);
        Assert.Equal(0, result.Gains.Matter);
        Assert.Equal(0, result.Gains.Energy);
        Assert.Equal(0, result.Gains.Data);
        Assert.Equal(3_600, result.Resources.Matter);
        Assert.Equal(2_160, result.Resources.Energy);
        Assert.Equal(1_260, result.Resources.Data);
    }

    [Fact]
    public async Task Resources_ClaimOffline_ReplaysIdenticalRetry()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("ReplayResource");
        await SetLastResourceClaimedAtUtcAsync(
            player.PlayerId,
            DateTimeOffset.UtcNow.AddHours(-3));
        var request = new ClaimOfflineResourcesRequest("resource-replay-request");

        var firstClaim = await _client.PostAsJsonAsync(
            $"/api/v1/players/{player.PlayerId}/resources/claim-offline",
            request);
        firstClaim.EnsureSuccessStatusCode();
        var firstResult = await firstClaim.Content.ReadFromJsonAsync<OfflineResourceClaimResponse>(JsonOptions.Default);

        var retryClaim = await _client.PostAsJsonAsync(
            $"/api/v1/players/{player.PlayerId}/resources/claim-offline",
            request);
        retryClaim.EnsureSuccessStatusCode();
        var retryResult = await retryClaim.Content.ReadFromJsonAsync<OfflineResourceClaimResponse>(JsonOptions.Default);

        Assert.NotNull(firstResult);
        Assert.NotNull(retryResult);
        Assert.Equal(firstResult.Gains.Matter, retryResult.Gains.Matter);
        Assert.Equal(firstResult.Gains.Energy, retryResult.Gains.Energy);
        Assert.Equal(firstResult.Gains.Data, retryResult.Gains.Data);
        Assert.Equal(firstResult.Resources.Matter, retryResult.Resources.Matter);
        Assert.Equal(firstResult.ClaimedAtUtc, retryResult.ClaimedAtUtc);
    }

    [Fact]
    public async Task Resources_ClaimOffline_RejectsReusedRequestIdWithDifferentPlayer()
    {
        await RegisterAndAuthorizeAsync(_client);
        var firstPlayer = await CreatePlayerAsync("ResourceFirst");
        var secondPlayer = await CreatePlayerAsync("ResourceSecond");
        var request = new ClaimOfflineResourcesRequest("resource-mismatch-request");

        var firstClaim = await _client.PostAsJsonAsync(
            $"/api/v1/players/{firstPlayer.PlayerId}/resources/claim-offline",
            request);
        firstClaim.EnsureSuccessStatusCode();

        var mismatchClaim = await _client.PostAsJsonAsync(
            $"/api/v1/players/{secondPlayer.PlayerId}/resources/claim-offline",
            request);

        Assert.Equal(HttpStatusCode.Conflict, mismatchClaim.StatusCode);

        var json = await mismatchClaim.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"idempotency_key_conflict\"", json);
    }

    [Fact]
    public async Task Resources_RejectCrossAccountAccess()
    {
        var ownerAuth = await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("ResourceOwner");
        var intruderAuth = await RegisterAccountAsync(_client, CreateUniqueEmail("resource-intruder"));
        Authorize(_client, intruderAuth.AccessToken);

        var response = await _client.GetAsync($"/api/v1/players/{player.PlayerId}/resources");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotEqual(ownerAuth.AccountId, intruderAuth.AccountId);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"player_forbidden\"", json);
    }

    [Fact]
    public async Task Purchases_ValidateSafeProduct_GrantsEntitlement()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("PurchaseNode");
        var request = CreatePurchaseRequest(
            player.PlayerId,
            "cosmetic_avatar_skin_neon",
            "tx-safe-001");

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/validate", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var purchase = await response.Content.ReadFromJsonAsync<ValidatePurchaseResponse>(JsonOptions.Default);
        Assert.NotNull(purchase);
        Assert.Equal(player.PlayerId, purchase.PlayerId);
        Assert.Equal("local_mock", purchase.Store);
        Assert.Equal("cosmetic_avatar_skin_neon", purchase.ProductId);
        Assert.Equal("cosmetic", purchase.ProductType);
        Assert.NotEqual(Guid.Empty, purchase.PurchaseReceiptId);
        Assert.NotEqual(Guid.Empty, purchase.EntitlementId);
    }

    [Fact]
    public async Task Purchases_RejectDuplicateReceipt()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("DuplicatePurchase");
        var request = CreatePurchaseRequest(
            player.PlayerId,
            "ui_theme_static",
            "tx-duplicate-001",
            "duplicate-original-request");

        var firstResponse = await _client.PostAsJsonAsync("/api/v1/purchases/validate", request);
        firstResponse.EnsureSuccessStatusCode();

        var duplicateResponse = await _client.PostAsJsonAsync(
            "/api/v1/purchases/validate",
            request with { RequestId = "duplicate-second-request" });

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);

        var json = await duplicateResponse.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"duplicate_receipt\"", json);
    }

    [Fact]
    public async Task Purchases_Validate_ReplaysIdenticalRetry()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("PurchaseReplay");
        var request = CreatePurchaseRequest(
            player.PlayerId,
            "profile_frame_founder",
            "tx-replay-001",
            "purchase-replay-request");

        var firstResponse = await _client.PostAsJsonAsync("/api/v1/purchases/validate", request);
        firstResponse.EnsureSuccessStatusCode();
        var firstPurchase = await firstResponse.Content.ReadFromJsonAsync<ValidatePurchaseResponse>(JsonOptions.Default);

        var retryResponse = await _client.PostAsJsonAsync("/api/v1/purchases/validate", request);
        retryResponse.EnsureSuccessStatusCode();
        var retryPurchase = await retryResponse.Content.ReadFromJsonAsync<ValidatePurchaseResponse>(JsonOptions.Default);

        Assert.NotNull(firstPurchase);
        Assert.NotNull(retryPurchase);
        Assert.Equal(firstPurchase.PurchaseReceiptId, retryPurchase.PurchaseReceiptId);
        Assert.Equal(firstPurchase.EntitlementId, retryPurchase.EntitlementId);
        Assert.Equal(firstPurchase.ValidatedAtUtc, retryPurchase.ValidatedAtUtc);
    }

    [Fact]
    public async Task Purchases_Validate_RejectsReusedRequestIdWithDifferentPayload()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("PurchaseMismatch");
        var request = CreatePurchaseRequest(
            player.PlayerId,
            "cosmetic_avatar_skin_neon",
            "tx-mismatch-001",
            "purchase-mismatch-request");

        var firstResponse = await _client.PostAsJsonAsync("/api/v1/purchases/validate", request);
        firstResponse.EnsureSuccessStatusCode();

        var mismatchRequest = CreatePurchaseRequest(
            player.PlayerId,
            "ui_theme_static",
            "tx-mismatch-002",
            "purchase-mismatch-request");
        var mismatchResponse = await _client.PostAsJsonAsync("/api/v1/purchases/validate", mismatchRequest);

        Assert.Equal(HttpStatusCode.Conflict, mismatchResponse.StatusCode);

        var json = await mismatchResponse.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"idempotency_key_conflict\"", json);
    }

    [Fact]
    public async Task Purchases_RejectUnknownProduct()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("UnknownProduct");
        var request = CreatePurchaseRequest(
            player.PlayerId,
            "unknown_product",
            "tx-unknown-001");

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/validate", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"unknown_product\"", json);
    }

    [Fact]
    public async Task Purchases_RejectPowerAffectingProduct()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("PowerProduct");
        var request = CreatePurchaseRequest(
            player.PlayerId,
            "combat_stat_boost",
            "tx-power-001");

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/validate", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"power_product_not_allowed\"", json);
    }

    [Fact]
    public async Task Purchases_RejectCrossAccountPlayer()
    {
        var ownerAuth = await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("PurchaseOwner");
        var intruderAuth = await RegisterAccountAsync(_client, CreateUniqueEmail("purchase-intruder"));
        Authorize(_client, intruderAuth.AccessToken);
        var request = CreatePurchaseRequest(
            player.PlayerId,
            "profile_frame_founder",
            "tx-cross-account-001");

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/validate", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotEqual(ownerAuth.AccountId, intruderAuth.AccountId);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"player_forbidden\"", json);
    }

    [Fact]
    public async Task ProtectedEndpoint_ReturnsAuthenticationError_WhenBearerTokenIsMissing()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/players",
            new CreatePlayerRequest("NoTokenNode"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"authentication_required\"", json);
    }

    [Fact]
    public async Task PlayerEndpoints_RejectCrossAccountAccess()
    {
        var firstAuth = await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("OwnerNode");
        var secondAuth = await RegisterAccountAsync(_client, CreateUniqueEmail("intruder"));
        Authorize(_client, secondAuth.AccessToken);

        var response = await _client.GetAsync($"/api/v1/players/{player.PlayerId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"player_forbidden\"", json);
        Assert.NotEqual(firstAuth.AccountId, secondAuth.AccountId);
    }

    [Fact]
    public async Task Auth_Login_ReturnsNewBearerSessionForExistingAccount()
    {
        var email = CreateUniqueEmail("login");
        await RegisterAccountAsync(_client, email);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(email, "CorrectHorseBattery1"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions.Default);
        Assert.NotNull(auth);
        Assert.NotEqual(Guid.Empty, auth.AccountId);
        Assert.False(string.IsNullOrWhiteSpace(auth.AccessToken));
        Assert.True(auth.ExpiresAtUtc > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Auth_Logout_RevokesCurrentBearerSession()
    {
        var auth = await RegisterAndAuthorizeAsync(_client);

        var logoutResponse = await _client.PostAsync("/api/v1/auth/logout", content: null);

        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);

        var logout = await logoutResponse.Content.ReadFromJsonAsync<LogoutResponse>(JsonOptions.Default);
        Assert.NotNull(logout);
        Assert.True(logout.RevokedAtUtc <= DateTimeOffset.UtcNow.AddSeconds(5));

        Authorize(_client, auth.AccessToken);
        var protectedResponse = await _client.PostAsJsonAsync(
            "/api/v1/players",
            new CreatePlayerRequest("RevokedNode"));

        Assert.Equal(HttpStatusCode.Unauthorized, protectedResponse.StatusCode);

        var json = await protectedResponse.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"authentication_required\"", json);
    }

    [Fact]
    public async Task Auth_Refresh_RotatesBearerSessionAndRejectsOldToken()
    {
        var originalAuth = await RegisterAndAuthorizeAsync(_client);

        var refreshResponse = await _client.PostAsync("/api/v1/auth/refresh", content: null);

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var refreshedAuth = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions.Default);
        Assert.NotNull(refreshedAuth);
        Assert.Equal(originalAuth.AccountId, refreshedAuth.AccountId);
        Assert.NotEqual(originalAuth.AccessToken, refreshedAuth.AccessToken);

        Authorize(_client, originalAuth.AccessToken);
        var oldTokenResponse = await _client.PostAsJsonAsync(
            "/api/v1/players",
            new CreatePlayerRequest("OldTokenNode"));

        Assert.Equal(HttpStatusCode.Unauthorized, oldTokenResponse.StatusCode);

        Authorize(_client, refreshedAuth.AccessToken);
        var newTokenResponse = await _client.PostAsJsonAsync(
            "/api/v1/players",
            new CreatePlayerRequest("NewTokenNode"));

        Assert.Equal(HttpStatusCode.Created, newTokenResponse.StatusCode);
    }

    [Fact]
    public async Task CloudSave_PersistsAcrossRestartedBackendHost()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"system-reboot-restart-{Guid.NewGuid():N}.db");
        Guid playerId;
        string accessToken;

        using (var firstFactory = TestBackendFactory.CreateWithDatabasePath(
            databasePath,
            deleteDatabaseOnDispose: false))
        {
            var firstClient = firstFactory.CreateClient();
            var auth = await RegisterAndAuthorizeAsync(firstClient);
            var player = await CreatePlayerAsync(firstClient, "DurableNode");
            playerId = player.PlayerId;
            accessToken = auth.AccessToken;
            var request = new UpsertCloudSaveRequest(
                SaveVersion: 2,
                SaveJson: """
                    {
                      "version": 2,
                      "systemReboot": {
                        "coreFragments": 1
                      }
                    }
                    """,
                Checksum: "restart-checksum",
                ClientSavedAtUtc: DateTimeOffset.Parse("2026-06-15T16:10:00Z"));

            var saveResponse = await firstClient.PutAsJsonAsync(
                $"/api/v1/cloud-saves/{player.PlayerId}",
                request);

            Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);
        }

        using (var secondFactory = TestBackendFactory.CreateWithDatabasePath(
            databasePath,
            deleteDatabaseOnDispose: true))
        {
            var secondClient = secondFactory.CreateClient();
            Authorize(secondClient, accessToken);
            var latestSave = await secondClient.GetFromJsonAsync<CloudSaveResponse>(
                $"/api/v1/cloud-saves/{playerId}",
                JsonOptions.Default);

            Assert.NotNull(latestSave);
            Assert.Equal(2, latestSave.SaveVersion);
            Assert.Equal("restart-checksum", latestSave.Checksum);
            Assert.Contains("\"coreFragments\": 1", latestSave.SaveJson);
        }
    }

    [Fact]
    public async Task CloudSave_RejectsInvalidJsonWithConsistentErrorEnvelope()
    {
        await RegisterAndAuthorizeAsync(_client);
        var player = await CreatePlayerAsync("GuardNode");
        var request = new UpsertCloudSaveRequest(
            SaveVersion: 1,
            SaveJson: "{ invalid json",
            Checksum: null,
            ClientSavedAtUtc: null);

        var response = await _client.PutAsJsonAsync(
            $"/api/v1/cloud-saves/{player.PlayerId}",
            request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"error\"", json);
        Assert.Contains("\"code\":\"invalid_save_json\"", json);
    }

    [Fact]
    public async Task GameConfig_ReturnsInitialBackendTuningValues()
    {
        var config = await _client.GetFromJsonAsync<GameConfigResponse>(
            "/api/v1/game-config",
            JsonOptions.Default);

        Assert.NotNull(config);
        Assert.Equal("system-reboot-config-v1", config.ConfigVersion);
        Assert.Equal(3, config.Combat.EarlyLoadoutSlots);
        Assert.Equal(7_200, config.OfflineProgress.EarlyOfflineCapSeconds);
        Assert.Contains(config.ResourceDefinitions, resource => resource.ResourceId == "core_fragments");
    }

    [Fact]
    public async Task GameConfig_LoadsActiveConfigFromDatabase()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"system-reboot-config-active-{Guid.NewGuid():N}.db");

        using var factory = TestBackendFactory.CreateWithDatabasePath(
            databasePath,
            deleteDatabaseOnDispose: true);
        var client = factory.CreateClient();
        await ReplaceActiveGameConfigAsync(databasePath, GameConfigDefaults.InitialConfig with
        {
            ConfigVersion = "test-config-v2",
            Resources = new ResourceGenerationConfig(
                MatterPerSecond: 2.0m,
                EnergyPerSecond: 1.0m,
                DataPerSecond: 0.5m),
            OfflineProgress = new OfflineProgressConfig(
                EarlyOfflineCapSeconds: 3_600,
                BaseOfflineEfficiency: 0.75m)
        });

        var config = await client.GetFromJsonAsync<GameConfigResponse>(
            "/api/v1/game-config",
            JsonOptions.Default);

        Assert.NotNull(config);
        Assert.Equal("test-config-v2", config.ConfigVersion);
        Assert.Equal(2.0m, config.Resources.MatterPerSecond);
        Assert.Equal(3_600, config.OfflineProgress.EarlyOfflineCapSeconds);
        Assert.Equal(0.75m, config.OfflineProgress.BaseOfflineEfficiency);
    }

    [Fact]
    public async Task GameConfig_PersistsAcrossRestartedBackendHost()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"system-reboot-config-{Guid.NewGuid():N}.db");

        using (var firstFactory = TestBackendFactory.CreateWithDatabasePath(
            databasePath,
            deleteDatabaseOnDispose: false))
        {
            var firstClient = firstFactory.CreateClient();
            var config = await firstClient.GetFromJsonAsync<GameConfigResponse>(
                "/api/v1/game-config",
                JsonOptions.Default);

            Assert.NotNull(config);
            Assert.Equal("system-reboot-config-v1", config.ConfigVersion);
        }

        using (var secondFactory = TestBackendFactory.CreateWithDatabasePath(
            databasePath,
            deleteDatabaseOnDispose: true))
        {
            var secondClient = secondFactory.CreateClient();
            var config = await secondClient.GetFromJsonAsync<GameConfigResponse>(
                "/api/v1/game-config",
                JsonOptions.Default);

            Assert.NotNull(config);
            Assert.Equal("system-reboot-config-v1", config.ConfigVersion);
            Assert.Equal(7_200, config.OfflineProgress.EarlyOfflineCapSeconds);
        }
    }

    private async Task<PlayerProfileResponse> CreatePlayerAsync(string displayName)
    {
        return await CreatePlayerAsync(_client, displayName);
    }

    private static async Task<PlayerProfileResponse> CreatePlayerAsync(
        HttpClient client,
        string displayName)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/players",
            new CreatePlayerRequest(displayName));

        response.EnsureSuccessStatusCode();

        var player = await response.Content.ReadFromJsonAsync<PlayerProfileResponse>(JsonOptions.Default);
        Assert.NotNull(player);
        return player;
    }

    private static async Task<AuthResponse> RegisterAndAuthorizeAsync(HttpClient client)
    {
        var auth = await RegisterAccountAsync(client, CreateUniqueEmail("account"));
        Authorize(client, auth.AccessToken);
        return auth;
    }

    private static async Task<AuthResponse> RegisterAccountAsync(HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterAccountRequest(email, "CorrectHorseBattery1"));

        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions.Default);
        Assert.NotNull(auth);
        Assert.NotEqual(Guid.Empty, auth.AccountId);
        Assert.False(string.IsNullOrWhiteSpace(auth.AccessToken));
        return auth;
    }

    private static void Authorize(HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    private static string CreateUniqueEmail(string prefix)
    {
        return $"{prefix}-{Guid.NewGuid():N}@example.test";
    }

    private static ValidatePurchaseRequest CreatePurchaseRequest(
        Guid playerId,
        string productId,
        string transactionId,
        string? requestId = null)
    {
        return new ValidatePurchaseRequest(
            requestId ?? $"request-{transactionId}",
            playerId,
            "local_mock",
            productId,
            $"local:{productId}:{transactionId}");
    }

    private async Task SetLastResourceClaimedAtUtcAsync(
        Guid playerId,
        DateTimeOffset lastResourceClaimedAtUtc)
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseSqlite($"Data Source={_factory.DatabasePath}")
            .Options;

        await using var dbContext = new GameDbContext(options);
        var player = await dbContext.Players.SingleAsync(player => player.PlayerId == playerId);
        player.LastResourceClaimedAtUtc = lastResourceClaimedAtUtc;
        await dbContext.SaveChangesAsync();
    }

    private static async Task ReplaceActiveGameConfigAsync(string databasePath, GameConfigResponse config)
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseSqlite($"Data Source={databasePath}")
            .Options;

        await using var dbContext = new GameDbContext(options);
        var activeConfigs = await dbContext.GameConfigVersions.ToListAsync();
        foreach (var activeConfig in activeConfigs)
        {
            activeConfig.IsActive = false;
            activeConfig.ActivatedAtUtc = null;
        }

        dbContext.GameConfigVersions.Add(new GameConfigVersionEntity
        {
            ConfigVersion = config.ConfigVersion,
            ConfigJson = JsonSerializer.Serialize(config, JsonOptions.Default),
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ActivatedAtUtc = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync();
    }

    private static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web);
    }
}
