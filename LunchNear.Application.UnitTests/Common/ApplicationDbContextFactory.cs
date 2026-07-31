namespace LunchNear.Application.UnitTests.Common;

using LunchNear.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Creates isolated EF Core InMemory-backed <see cref="ApplicationDbContext"/> instances so
/// handler tests exercise the exact same LINQ/EF configuration as production, without a real
/// SQLite file. Each call with a fresh database name gives a clean, isolated database; reusing
/// the same name across multiple factory calls lets a test simulate separate request scopes
/// reading/writing the same underlying data (e.g. to prove a handler doesn't rely on another
/// handler's in-memory change tracker state).
/// </summary>
public static class ApplicationDbContextFactory
{
    public static ApplicationDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
