namespace LunchNear.Api.Endpoints;

using global::Mediator;
using LunchNear.Application.StudentDiscounts.Queries;

public static class StudentDiscountEndpoints
{
    public static IEndpointRouteBuilder MapStudentDiscountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/restaurants/{restaurantId:int}/studentdiscounts").WithTags("StudentDiscounts");

        group.MapGet("", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetStudentDiscountsQuery(restaurantId), ct));

        group.MapGet("exists", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new HasStudentDiscountQuery(restaurantId), ct));

        return app;
    }
}
