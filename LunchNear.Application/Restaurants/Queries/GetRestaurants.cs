namespace LunchNear.Application.Restaurants.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Restaurants;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetRestaurantsQuery : IRequest<IReadOnlyList<RestaurantDto>>;

public sealed class GetRestaurantsQueryHandler : IRequestHandler<GetRestaurantsQuery, IReadOnlyList<RestaurantDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRestaurantsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<RestaurantDto>> Handle(GetRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var restaurants = await _context.Restaurants
            .Include(r => r.StudentDiscounts)
            .Include(r => r.LunchDeals)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return restaurants.Adapt<List<RestaurantDto>>();
    }
}
