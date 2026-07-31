namespace LunchNear.Api.Endpoints;

using global::Mediator;
using LunchNear.Application.LunchDeals.Queries;

public static class LunchDealEndpoints
{
    public static IEndpointRouteBuilder MapLunchDealEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/restaurants/{restaurantId:int}/lunchdeals", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
                await mediator.Send(new GetLunchDealsByRestaurantQuery(restaurantId), ct))
            .WithTags("LunchDeals");

        app.MapGet("api/lunchdeals", async (IMediator mediator, CancellationToken ct) =>
                await mediator.Send(new GetAllActiveLunchDealsQuery(), ct))
            .WithTags("LunchDeals");

        return app;
    }
}
