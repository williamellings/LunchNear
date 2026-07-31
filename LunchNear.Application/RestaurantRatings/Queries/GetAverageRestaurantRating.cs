namespace LunchNear.Application.RestaurantRatings.Queries;

using global::Mediator;
using LunchNear.Application.Common.Exceptions;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed record GetAverageRestaurantRatingQuery(int RestaurantId) : IRequest<decimal>;

public sealed class GetAverageRestaurantRatingQueryHandler : IRequestHandler<GetAverageRestaurantRatingQuery, decimal>
{
    private readonly IApplicationDbContext _context;

    public GetAverageRestaurantRatingQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<decimal> Handle(GetAverageRestaurantRatingQuery request, CancellationToken cancellationToken)
    {
        var averageRating = await _context.Restaurants
            .Where(r => r.Id == request.RestaurantId)
            .Select(r => (decimal?)r.AverageRating)
            .FirstOrDefaultAsync(cancellationToken);

        return averageRating ?? throw new NotFoundException(nameof(Restaurant), request.RestaurantId);
    }
}
