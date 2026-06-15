using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Game.Backend.Modules.CloudSaves;
using Game.Backend.Modules.GameConfig;
using Game.Backend.Modules.Players;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Game.Backend.Tests;

public sealed class BackendApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BackendApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePlayer_ReturnsUnityFriendlyProfileContract()
    {
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
    public async Task CloudSave_RejectsInvalidJsonWithConsistentErrorEnvelope()
    {
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
        var response = await _client.PostAsJsonAsync(
            "/api/v1/players",
            new CreatePlayerRequest(displayName));

        response.EnsureSuccessStatusCode();

        var player = await response.Content.ReadFromJsonAsync<PlayerProfileResponse>(JsonOptions.Default);
        Assert.NotNull(player);
        return player;
    }

    private static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web);
    }
}
