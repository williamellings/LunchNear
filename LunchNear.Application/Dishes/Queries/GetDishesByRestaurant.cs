namespace LunchNear.Application.Dishes.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Dishes;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetDishesByRestaurantQuery(int RestaurantId) : IRequest<IReadOnlyList<DishDto>>;

/// <summary>
/// Reads the cached AverageRating/RatingsCount columns directly - a single query, no per-dish
/// aggregation. This is what actually fixes the N+1 problem found in the previous review
/// (the old service recomputed each dish's average with a separate query inside a loop).
/// </summary>
public sealed class GetDishesByRestaurantQueryHandler : IRequestHandler<GetDishesByRestaurantQuery, IReadOnlyList<DishDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDishesByRestaurantQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<DishDto>> Handle(GetDishesByRestaurantQuery request, CancellationToken cancellationToken)
    {
        var dishes = await _context.Dishes
            .Where(d => d.RestaurantId == request.RestaurantId)
            .AsNoTracking()
            .OrderByDescending(d => d.AverageRating)
            .ToListAsync(cancellationToken);

        return dishes.Adapt<List<DishDto>>();
    }
}
