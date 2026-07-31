namespace LunchNear.Api.Endpoints;

using global::Mediator;
using LunchNear.Application.DishRatings.Commands;
using LunchNear.Application.DishRatings.Queries;
using LunchNear.Contracts.Ratings;

public static class DishRatingEndpoints
{
    public static IEndpointRouteBuilder MapDishRatingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/dishes/{dishId:int}/ratings").WithTags("DishRatings");

        group.MapGet("", async (int dishId, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetDishRatingsQuery(dishId), ct));

        group.MapGet("average", async (int dishId, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetAverageDishRatingQuery(dishId), ct));

        group.MapPost("", async (int dishId, SubmitRatingRequest request, IMediator mediator, CancellationToken ct) =>
        {
            var dto = await mediator.Send(
                new SubmitDishRatingCommand(dishId, request.UserId, request.Rating, request.Review), ct);
            return Results.Created($"/api/dishes/{dishId}/ratings", dto);
        });

        return app;
    }
}
