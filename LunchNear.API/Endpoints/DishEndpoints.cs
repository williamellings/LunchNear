namespace LunchNear.Api.Endpoints;

using global::Mediator;
using LunchNear.Application.Dishes.Queries;

public static class DishEndpoints
{
    public static IEndpointRouteBuilder MapDishEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/restaurants/{restaurantId:int}/dishes", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
                await mediator.Send(new GetDishesByRestaurantQuery(restaurantId), ct))
            .WithTags("Dishes");

        app.MapGet("api/restaurants/{restaurantId:int}/dishes/top-rated", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
                await mediator.Send(new GetTopRatedDishesQuery(restaurantId), ct))
            .WithTags("Dishes");

        app.MapGet("api/dishes/{dishId:int}", async (int dishId, IMediator mediator, CancellationToken ct) =>
            {
                var dish = await mediator.Send(new GetDishByIdQuery(dishId), ct);
                return dish is not null ? Results.Ok(dish) : Results.NotFound();
            })
            .WithTags("Dishes");

        return app;
    }
}
