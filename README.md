<p align="center">
  <img height="500" src="https://github.com/user-attachments/assets/503680f5-065b-46c5-a4b8-2a6d8fed2286" alt="Lunch Deals" />
  &nbsp;&nbsp;
  <img height="500" src="https://github.com/user-attachments/assets/ee59b2d9-0657-4ac3-af41-be1aa7a58209" alt="LunchNear Landing" />
</p>



- **Restaurant discovery** — search, filter, and find nearby places using browser geolocation
- **Real venue data** — restaurants seeded from OpenStreetMap (Overpass API) in central Gothenburg
- **Lunch deals** — time-windowed offers with price, days of week, and active hours
- **Student discounts** — admin-curated only; a badge appears on a restaurant after a discount is added
- **Independent ratings** — restaurant ratings and dish ratings are separate aggregates
- **Admin panel** — add student discounts and lunch deals at `/admin`
- **PWA** — installable Blazor WebAssembly app with offline service worker support
- **Clean Architecture** — Domain, Application, Infrastructure, API, and Web layers with CQRS via Mediator

## Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend | Blazor WebAssembly, PWA |
| API | ASP.NET Core Minimal API |
| Application | Mediator (CQRS), FluentValidation, Mapster |
| Domain | Rich domain model, value objects, aggregate roots |
| Persistence | EF Core 10, SQLite |
| Orchestration | .NET Aspire AppHost (optional) |
| Tests | xUnit — domain, application, and API integration tests |

**Target framework:** .NET 10

## Solution Structure

```
LunchNear.slnx
├── LunchNear.Domain              Entities, value objects, domain rules (no dependencies)
├── LunchNear.Application         Commands, queries, validators, mapping
├── LunchNear.Infrastructure      EF Core, migrations, OSM seeding
├── LunchNear.Contracts           Shared DTOs between API and Web
├── LunchNear.API                 HTTP endpoints, DI composition root
├── LunchNear.Web                 Blazor WASM UI (references Contracts only)
├── LunchNear.AppHost             .NET Aspire orchestrator (optional)
├── LunchNear.ServiceDefaults     OpenTelemetry, health checks
└── *Tests                        Unit and integration tests
```

### Dependency flow

```
Web / API  →  Contracts  →  Application  →  Domain
                                ↑
                         Infrastructure
```

The Web project never references Domain or Application — it communicates with the API over HTTP using shared DTOs from Contracts.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PowerShell (for the dev startup script on Windows)
- A modern browser with geolocation support (optional, for nearby search)

## Getting Started

### Quick start (recommended)

From the repository root:

```powershell
.\run-dev.ps1
```

This script:

1. Frees ports `5022` and `5102` if they are in use
2. Builds the API and Web projects once
3. Starts both in separate terminal windows

| Service | URL |
|---------|-----|
| Web app | http://localhost:5102 |
| API | http://localhost:5022 |
| Admin | http://localhost:5102/admin |
| OpenAPI (dev) | http://localhost:5022/openapi/v1.json |

If the page stays on "Loading", hard-refresh once with `Ctrl + Shift + R`.

### Manual start (two terminals)

**Terminal 1 — API:**

```powershell
dotnet run --project LunchNear.API\LunchNear.API.csproj --launch-profile http
```

**Terminal 2 — Web:**

```powershell
dotnet run --project LunchNear.Web\LunchNear.Web.csproj --launch-profile http
```

The Web client reads the API base URL from `LunchNear.Web/wwwroot/appsettings.json`:

```json
{
  "ApiBaseUrl": "http://localhost:5022"
}
```

> **Note:** For local development, prefer `run-dev.ps1` or the two-terminal approach above. Running through Aspire AppHost can cause Blazor WASM loading issues in some setups.

## Admin Panel

Navigate to **http://localhost:5102/admin** to:

1. Select a restaurant
2. Add a **student discount** (description + percentage) — the green badge on the restaurant list appears only after you save one here
3. Add a **lunch deal** (name, price, time window, days of week)

Student discounts are **never auto-generated** during database seeding — they are intentionally admin-curated.

## API Overview

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/restaurants` | List all restaurants |
| GET | `/api/restaurants/search?query=` | Search by name or address |
| GET | `/api/restaurants/nearby?latitude=&longitude=&radiusKm=` | Nearby restaurants |
| POST | `/api/restaurants/filter` | Filter by criteria |
| GET | `/api/restaurants/{id}` | Restaurant details |
| GET | `/api/restaurants/{id}/dishes` | Dishes for a restaurant |
| GET | `/api/restaurants/{id}/lunchdeals` | Lunch deals for a restaurant |
| POST | `/api/restaurants/{id}/lunchdeals` | Create a lunch deal |
| GET | `/api/restaurants/{id}/studentdiscounts` | Student discounts |
| POST | `/api/restaurants/{id}/studentdiscounts` | Create a student discount |
| POST | `/api/restaurants/{id}/ratings` | Rate a restaurant |
| POST | `/api/dishes/{id}/ratings` | Rate a dish |

Errors are returned as [RFC 7807 Problem Details](https://datatracker.ietf.org/doc/html/rfc7807).

## Architecture Highlights

### Domain layer

- **Aggregate roots:** `Restaurant` (discounts, deals, restaurant ratings) and `Dish` (dish ratings) are separate aggregates
- **Value objects:** `GeoLocation` (with Haversine distance), `RatingValue` (1–5 stars)
- **Rich model:** private setters, factory methods (`Create`), and behavior methods (`AddRating`, `AddLunchDeal`)

### Application layer

- **CQRS:** commands (writes) and queries (reads) in separate folders
- **Mediator pipeline:** validation, logging, and exception handling as cross-cutting behaviors
- **No generic Repository:** handlers use `IApplicationDbContext` directly — EF Core's `DbSet` already acts as repository and unit of work

### Infrastructure layer

- SQLite database with EF Core migrations
- Startup seeding from OpenStreetMap Overpass API (real Gothenburg venues)
- Fallback demo data when Overpass is unreachable
- Auditable entity interceptor for `CreatedAt` timestamps

### Web layer

- Blazor WASM SPA referencing **Contracts only**
- Typed `LunchNearApiClient` for all API calls
- Browser geolocation for nearby restaurant search
- PWA manifest and service worker

## Running Tests

```powershell
dotnet test LunchNear.slnx
```

Or run individual test projects:

```powershell
dotnet test LunchNear.Domain.UnitTests
dotnet test LunchNear.Application.UnitTests
dotnet test LunchNear.Api.IntegrationTests
```

## Database

The API uses SQLite. On first startup, EF Core applies migrations and seeds demo data automatically.

To reset the database, delete the SQLite file in the API output directory and restart the API:

```
LunchNear.API/bin/Debug/net10.0/lunchnear.db
```

## Configuration

| Setting | Location | Default |
|---------|----------|---------|
| API base URL (Web) | `LunchNear.Web/wwwroot/appsettings.json` | `http://localhost:5022` |
| CORS origins (API) | `LunchNear.API/appsettings.json` | Allow any origin in dev |
| Connection string | `LunchNear.API/appsettings.json` | SQLite file |

## License

See the repository for license details.
