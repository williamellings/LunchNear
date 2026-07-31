namespace LunchNear.Application.Dishes.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Dishes;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetTopRatedDishesQuery(int RestaurantId, int Take = 5) : IRequest<IReadOnlyList<DishDto>>;

public sealed class GetTopRatedDishesQueryHandler : IRequestHandler<GetTopRatedDishesQuery, IReadOnlyList<DishDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTopRatedDishesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<DishDto>> Handle(GetTopRatedDishesQuery request, CancellationToken cancellationToken)
    {
        var dishes = await _context.Dishes
            .Where(d => d.RestaurantId == request.RestaurantId && d.RatingsCount > 0)
            .AsNoTracking()
            .OrderByDescending(d => d.AverageRating)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        return dishes.Adapt<List<DishDto>>();
    }
}
