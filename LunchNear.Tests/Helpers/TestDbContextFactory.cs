namespace LunchNear.Tests.Helpers;

using Microsoft.EntityFrameworkCore;
using LunchNear.API.Data;

public static class TestDbContextFactory
{
    public static LunchNearDbContext CreateTestDbContext()
    {
        var options = new DbContextOptionsBuilder<LunchNearDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new LunchNearDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
