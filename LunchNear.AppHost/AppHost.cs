var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.LunchNear_API>("api")
    .WithExternalHttpEndpoints();

// LunchNear.Web is Blazor WASM: code executing in the browser cannot resolve Aspire service
// discovery, so its API base address is configured explicitly via wwwroot/appsettings.json.
// WithExternalHttpEndpoints ensures the Aspire proxy forwards Blazor _framework assets correctly.
builder.AddProject<Projects.LunchNear_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
