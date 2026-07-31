namespace LunchNear.Api.Endpoints;

using global::Mediator;
using LunchNear.Application.RestaurantRatings.Commands;
using LunchNear.Application.RestaurantRatings.Queries;
using LunchNear.Contracts.Ratings;

public static class RestaurantRatingEndpoints
{
    public static IEndpointRouteBuilder MapRestaurantRatingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/restaurants/{restaurantId:int}/ratings").WithTags("RestaurantRatings");

        group.MapGet("", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetRestaurantRatingsQuery(restaurantId), ct));

        group.MapGet("average", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetAverageRestaurantRatingQuery(restaurantId), ct));

        group.MapPost("", async (int restaurantId, SubmitRatingRequest request, IMediator mediator, CancellationToken ct) =>
        {
            var dto = await mediator.Send(
                new SubmitRestaurantRatingCommand(restaurantId, request.UserId, request.Rating, request.Review), ct);
            return Results.Created($"/api/restaurants/{restaurantId}/ratings", dto);
        });

        return app;
    }
}
