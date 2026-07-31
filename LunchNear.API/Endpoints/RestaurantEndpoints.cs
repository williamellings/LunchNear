namespace LunchNear.Api.Endpoints;

using global::Mediator;
using LunchNear.Application.Restaurants.Queries;
using LunchNear.Contracts.Restaurants;

public static class RestaurantEndpoints
{
    public static IEndpointRouteBuilder MapRestaurantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/restaurants").WithTags("Restaurants");

        group.MapGet("", async (IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetRestaurantsQuery(), ct));

        group.MapGet("search", async (string? query, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new SearchRestaurantsQuery(query), ct));

        group.MapGet("nearby", async (double latitude, double longitude, IMediator mediator, CancellationToken ct, double radiusKm = 5) =>
            await mediator.Send(new GetNearbyRestaurantsQuery(latitude, longitude, radiusKm), ct));

        group.MapPost("filter", async (RestaurantFilterRequest criteria, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new FilterRestaurantsQuery(criteria), ct));

        group.MapGet("{id:int}", async (int id, IMediator mediator, CancellationToken ct) =>
        {
            var restaurant = await mediator.Send(new GetRestaurantByIdQuery(id), ct);
            return restaurant is not null ? Results.Ok(restaurant) : Results.NotFound();
        });

        return app;
    }
}
