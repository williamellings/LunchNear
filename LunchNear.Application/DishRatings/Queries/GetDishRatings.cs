namespace LunchNear.Application.DishRatings.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Ratings;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetDishRatingsQuery(int DishId) : IRequest<IReadOnlyList<DishRatingDto>>;

public sealed class GetDishRatingsQueryHandler : IRequestHandler<GetDishRatingsQuery, IReadOnlyList<DishRatingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDishRatingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<DishRatingDto>> Handle(GetDishRatingsQuery request, CancellationToken cancellationToken)
    {
        // Ordered by Id (equivalent to insertion/creation order for this append-only list)
        // rather than CreatedAt directly - the SQLite provider cannot translate ORDER BY on a
        // DateTimeOffset column, and Id avoids that provider limitation entirely.
        var ratings = await _context.DishRatings
            .Where(r => r.DishId == request.DishId)
            .AsNoTracking()
            .OrderByDescending(r => r.Id)
            .ToListAsync(cancellationToken);

        return ratings.Adapt<List<DishRatingDto>>();
    }
}
