namespace LunchNear.Application.RestaurantRatings.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Ratings;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetRestaurantRatingsQuery(int RestaurantId) : IRequest<IReadOnlyList<RestaurantRatingDto>>;

public sealed class GetRestaurantRatingsQueryHandler : IRequestHandler<GetRestaurantRatingsQuery, IReadOnlyList<RestaurantRatingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRestaurantRatingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<RestaurantRatingDto>> Handle(
        GetRestaurantRatingsQuery request,
        CancellationToken cancellationToken)
    {
        // Ordered by Id (equivalent to insertion/creation order for this append-only list)
        // rather than CreatedAt directly - the SQLite provider cannot translate ORDER BY on a
        // DateTimeOffset column, and Id avoids that provider limitation entirely.
        var ratings = await _context.RestaurantRatings
            .Where(r => r.RestaurantId == request.RestaurantId)
            .AsNoTracking()
            .OrderByDescending(r => r.Id)
            .ToListAsync(cancellationToken);

        return ratings.Adapt<List<RestaurantRatingDto>>();
    }
}
