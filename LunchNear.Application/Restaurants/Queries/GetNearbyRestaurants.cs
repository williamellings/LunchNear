namespace LunchNear.Application.Restaurants.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Restaurants;
using LunchNear.Domain.ValueObjects;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetNearbyRestaurantsQuery(double Latitude, double Longitude, double RadiusKm = 5)
    : IRequest<IReadOnlyList<NearbyRestaurantDto>>;

public sealed class GetNearbyRestaurantsQueryHandler
    : IRequestHandler<GetNearbyRestaurantsQuery, IReadOnlyList<NearbyRestaurantDto>>
{
    private readonly IApplicationDbContext _context;

    public GetNearbyRestaurantsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<NearbyRestaurantDto>> Handle(
        GetNearbyRestaurantsQuery request,
        CancellationToken cancellationToken)
    {
        var origin = GeoLocation.Create(request.Latitude, request.Longitude);

        var restaurants = await _context.Restaurants
            .Include(r => r.StudentDiscounts)
            .Include(r => r.LunchDeals)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return restaurants
            .Select(r => new NearbyRestaurantDto(r.Adapt<RestaurantDto>(), origin.DistanceToKm(r.Location)))
            .Where(n => n.DistanceKm <= request.RadiusKm)
            .OrderBy(n => n.DistanceKm)
            .ToList();
    }
}
