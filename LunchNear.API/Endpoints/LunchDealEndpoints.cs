namespace LunchNear.Api.Endpoints;

using global::Mediator;
using LunchNear.Application.LunchDeals.Commands;
using LunchNear.Application.LunchDeals.Queries;
using LunchNear.Contracts.LunchDeals;

public static class LunchDealEndpoints
{
    public static IEndpointRouteBuilder MapLunchDealEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/restaurants/{restaurantId:int}/lunchdeals", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
                await mediator.Send(new GetLunchDealsByRestaurantQuery(restaurantId), ct))
            .WithTags("LunchDeals");

        app.MapPost("api/restaurants/{restaurantId:int}/lunchdeals", async (
                int restaurantId,
                CreateLunchDealRequest request,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var dto = await mediator.Send(
                    new AddLunchDealCommand(
                        restaurantId,
                        request.Name,
                        request.Description,
                        request.Price,
                        request.StartTime,
                        request.EndTime,
                        request.DaysOfWeek), ct);
                return Results.Created($"/api/restaurants/{restaurantId}/lunchdeals/{dto.Id}", dto);
            })
            .WithTags("LunchDeals");

        app.MapGet("api/lunchdeals", async (IMediator mediator, CancellationToken ct) =>
                await mediator.Send(new GetAllActiveLunchDealsQuery(), ct))
            .WithTags("LunchDeals");

        return app;
    }
}
