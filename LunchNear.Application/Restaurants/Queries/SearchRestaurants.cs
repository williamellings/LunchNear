namespace LunchNear.Application.Restaurants.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Restaurants;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record SearchRestaurantsQuery(string? Query) : IRequest<IReadOnlyList<RestaurantDto>>;

public sealed class SearchRestaurantsQueryHandler : IRequestHandler<SearchRestaurantsQuery, IReadOnlyList<RestaurantDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchRestaurantsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<RestaurantDto>> Handle(SearchRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Restaurants
            .Include(r => r.StudentDiscounts)
            .Include(r => r.LunchDeals)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var term = request.Query.Trim();
            query = query.Where(r => EF.Functions.Like(r.Name, $"%{term}%"));
        }

        var restaurants = await query.ToListAsync(cancellationToken);
        return restaurants.Adapt<List<RestaurantDto>>();
    }
}
