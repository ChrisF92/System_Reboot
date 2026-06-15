using System.Text.Json;
using Game.Backend.Middleware;
using Game.Backend.Modules.CloudSaves;
using Game.Backend.Modules.GameConfig;
using Game.Backend.Modules.Players;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PlayerRepository>();
builder.Services.AddSingleton<PlayerService>();
builder.Services.AddSingleton<CloudSaveRepository>();
builder.Services.AddSingleton<CloudSaveService>();
builder.Services.AddSingleton<GameConfigService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
