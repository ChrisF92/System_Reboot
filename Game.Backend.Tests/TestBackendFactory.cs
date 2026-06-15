using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Game.Backend.Tests;

public sealed class TestBackendFactory : WebApplicationFactory<Program>
{
    public TestBackendFactory()
        : this(CreateDatabasePath(), deleteDatabaseOnDispose: true)
    {
    }

    public TestBackendFactory(string databasePath, bool deleteDatabaseOnDispose = true)
    {
        DatabasePath = databasePath;
        _deleteDatabaseOnDispose = deleteDatabaseOnDispose;
    }

    private readonly bool _deleteDatabaseOnDispose;

    public string DatabasePath { get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:GameDatabase"] = $"Data Source={DatabasePath}"
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && _deleteDatabaseOnDispose)
        {
            DeleteDatabaseFile(DatabasePath);
            DeleteDatabaseFile($"{DatabasePath}-shm");
            DeleteDatabaseFile($"{DatabasePath}-wal");
        }
    }

    private static string CreateDatabasePath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            $"system-reboot-tests-{Guid.NewGuid():N}.db");
    }

    private static void DeleteDatabaseFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
