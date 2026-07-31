var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.LunchNear_Api>("api");

// LunchNear.Web is Blazor WASM: code executing in the browser cannot resolve Aspire service
// discovery, so its API base address is configured explicitly via wwwroot/appsettings.json.
// WithReference/WaitFor here still gives us correct startup ordering and a single dashboard
// view (logs/traces/health) for both services during local development.
builder.AddProject<Projects.LunchNear_Web>("web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
