using System.Globalization;
using LunchNear.Api.Endpoints;
using LunchNear.Api.Infrastructure;
using LunchNear.Application;
using LunchNear.Application.Common.Behaviours;
using LunchNear.Infrastructure;
using LunchNear.Infrastructure.Persistence;
using Mediator;

// EF Core's SQLite provider stores `decimal` as TEXT and parses it back with the current
// thread culture when comparing/sorting. Without forcing invariant culture, this throws on any
// machine whose locale uses a non-'.' decimal separator (e.g. ORDER BY on AverageRating).
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMediator(options =>
{
    // Handlers depend on the scoped IApplicationDbContext, so the mediator and its handlers
    // must be Scoped rather than the library's default Singleton lifetime.
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.PipelineBehaviors =
    [
        typeof(UnhandledExceptionBehaviour<,>),
        typeof(ValidationBehaviour<,>),
        typeof(LoggingBehaviour<,>)
    ];
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebClient", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

        if (allowedOrigins is { Length: > 0 })
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("WebClient");

app.MapRestaurantEndpoints();
app.MapDishEndpoints();
app.MapRestaurantRatingEndpoints();
app.MapDishRatingEndpoints();
app.MapStudentDiscountEndpoints();
app.MapLunchDealEndpoints();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbContextSeeder>();
    await seeder.SeedAsync();
}

app.Run();

/// <summary>Entry point class made visible for WebApplicationFactory-based integration tests.</summary>
public partial class Program
{
}
