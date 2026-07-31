using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LunchNear.Web;
using LunchNear.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// wwwroot/appsettings.json (+ appsettings.{Environment}.json) is loaded automatically by
// WebAssemblyHostBuilder. Aspire service discovery cannot reach code running in the browser,
// so the API base address is configured explicitly here instead.
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is not configured in wwwroot/appsettings.json.");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});
builder.Services.AddScoped<LunchNearApiClient>();
builder.Services.AddScoped<GeolocationService>();
builder.Services.AddScoped<UserIdentityService>();

await builder.Build().RunAsync();
