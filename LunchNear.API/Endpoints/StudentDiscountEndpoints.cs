namespace LunchNear.Api.Endpoints;

using global::Mediator;
using LunchNear.Application.StudentDiscounts.Commands;
using LunchNear.Application.StudentDiscounts.Queries;
using LunchNear.Contracts.StudentDiscounts;

public static class StudentDiscountEndpoints
{
    public static IEndpointRouteBuilder MapStudentDiscountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/restaurants/{restaurantId:int}/studentdiscounts").WithTags("StudentDiscounts");

        group.MapGet("", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetStudentDiscountsQuery(restaurantId), ct));

        group.MapGet("exists", async (int restaurantId, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new HasStudentDiscountQuery(restaurantId), ct));

        group.MapPost("", async (int restaurantId, CreateStudentDiscountRequest request, IMediator mediator, CancellationToken ct) =>
        {
            var dto = await mediator.Send(
                new AddStudentDiscountCommand(restaurantId, request.Description, request.DiscountPercentage), ct);
            return Results.Created($"/api/restaurants/{restaurantId}/studentdiscounts/{dto.Id}", dto);
        });

        return app;
    }
}
