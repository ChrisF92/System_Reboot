using System.Text.Json;
using Game.Backend.Database;
using Game.Backend.Middleware;
using Game.Backend.Modules.Auth;
using Game.Backend.Modules.CloudSaves;
using Game.Backend.Modules.GameConfig;
using Game.Backend.Modules.Players;
using Game.Backend.Modules.Purchases;
using Game.Backend.Modules.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddDbContext<GameDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("GameDatabase")
        ?? throw new InvalidOperationException("Connection string 'GameDatabase' is required.");

    options.UseSqlite(connectionString);
});
builder.Services.AddAuthentication(BearerSessionAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, BearerSessionAuthenticationHandler>(
        BearerSessionAuthenticationHandler.SchemeName,
        _ => { });
builder.Services.AddAuthorization();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<TokenHasher>();
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<CloudSaveRepository>();
builder.Services.AddScoped<CloudSaveService>();
builder.Services.AddScoped<ResourceRepository>();
builder.Services.AddScoped<ResourceService>();
builder.Services.AddScoped<PurchaseRepository>();
builder.Services.AddScoped<PurchaseService>();
builder.Services.AddSingleton<PurchaseProductCatalog>();
builder.Services.AddSingleton<MockPurchaseReceiptValidator>();
builder.Services.AddSingleton<GameConfigService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
