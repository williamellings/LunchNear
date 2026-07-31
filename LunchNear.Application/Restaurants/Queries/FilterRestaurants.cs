namespace LunchNear.Application.Restaurants.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Restaurants;
using LunchNear.Domain.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record FilterRestaurantsQuery(RestaurantFilterRequest Criteria) : IRequest<IReadOnlyList<RestaurantDto>>;

public sealed class FilterRestaurantsQueryHandler : IRequestHandler<FilterRestaurantsQuery, IReadOnlyList<RestaurantDto>>
{
    private readonly IApplicationDbContext _context;

    public FilterRestaurantsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<RestaurantDto>> Handle(FilterRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Restaurants
            .Include(r => r.StudentDiscounts)
            .Include(r => r.LunchDeals)
            .AsNoTracking()
            .AsQueryable();

        var criteria = request.Criteria;

        if (!string.IsNullOrWhiteSpace(criteria.PriceRange) &&
            Enum.TryParse<PriceRange>(criteria.PriceRange, ignoreCase: true, out var priceRange))
        {
            query = query.Where(r => r.PriceRange == priceRange);
        }

        if (criteria.HasStudentDiscount.HasValue)
        {
            query = criteria.HasStudentDiscount.Value
                ? query.Where(r => r.StudentDiscounts.Any())
                : query.Where(r => !r.StudentDiscounts.Any());
        }

        if (criteria.HasLunchDeals.HasValue)
        {
            query = criteria.HasLunchDeals.Value
                ? query.Where(r => r.LunchDeals.Any())
                : query.Where(r => !r.LunchDeals.Any());
        }

        var restaurants = await query.ToListAsync(cancellationToken);
        return restaurants.Adapt<List<RestaurantDto>>();
    }
}
