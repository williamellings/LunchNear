namespace LunchNear.Application.DishRatings.Queries;

using global::Mediator;
using LunchNear.Application.Common.Exceptions;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed record GetAverageDishRatingQuery(int DishId) : IRequest<decimal>;

public sealed class GetAverageDishRatingQueryHandler : IRequestHandler<GetAverageDishRatingQuery, decimal>
{
    private readonly IApplicationDbContext _context;

    public GetAverageDishRatingQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<decimal> Handle(GetAverageDishRatingQuery request, CancellationToken cancellationToken)
    {
        var averageRating = await _context.Dishes
            .Where(d => d.Id == request.DishId)
            .Select(d => (decimal?)d.AverageRating)
            .FirstOrDefaultAsync(cancellationToken);

        return averageRating ?? throw new NotFoundException(nameof(Dish), request.DishId);
    }
}
