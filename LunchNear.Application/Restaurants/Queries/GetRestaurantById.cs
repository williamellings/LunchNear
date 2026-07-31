namespace LunchNear.Application.Restaurants.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Restaurants;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetRestaurantByIdQuery(int Id) : IRequest<RestaurantDto?>;

public sealed class GetRestaurantByIdQueryHandler : IRequestHandler<GetRestaurantByIdQuery, RestaurantDto?>
{
    private readonly IApplicationDbContext _context;

    public GetRestaurantByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<RestaurantDto?> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants
            .Include(r => r.StudentDiscounts)
            .Include(r => r.LunchDeals)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        return restaurant?.Adapt<RestaurantDto>();
    }
}
