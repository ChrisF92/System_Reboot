using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Game.Backend.Modules.Auth;
using Game.Backend.Modules.CloudSaves;
using Game.Backend.Modules.GameConfig;
using Game.Backend.Modules.Players;

namespace Game.Backend.Tests;

public sealed class BackendApiTests : IClassFixture<TestBackendFactory>
{
    private readonly HttpClient _client;

    public BackendApiTests(TestBackendFactory factory)
    {
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

    private static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web);
    }
}
