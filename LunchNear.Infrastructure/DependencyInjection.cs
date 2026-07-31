namespace LunchNear.Infrastructure;

using LunchNear.Application.Common.Interfaces;
using LunchNear.Infrastructure.Persistence;
using LunchNear.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AuditableEntitySaveChangesInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? $"Data Source={Path.Combine(AppContext.BaseDirectory, "lunchnear.db")}";

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
            options
                .UseSqlite(connectionString)
                .AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>()));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddHttpClient("Overpass", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("LunchNearApp/1.0 (+https://github.com/lunchnear)");
        });

        services.AddScoped<ApplicationDbContextSeeder>();

        return services;
    }
}
