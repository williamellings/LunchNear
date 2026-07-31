namespace LunchNear.Api.IntegrationTests;

using LunchNear.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Boots the real LunchNear.Api composition root (Program.cs) end to end, swapping the
/// production SQLite file for a SQLite in-memory database kept alive via a single open
/// connection. Real SQLite (rather than the EF InMemory provider) is used because the app's
/// startup seeder calls Database.MigrateAsync(), which the EF InMemory provider does not
/// support - this way integration tests exercise the exact same migrations as production.
/// </summary>
public sealed class LunchNearApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
