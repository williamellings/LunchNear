namespace LunchNear.Application.LunchDeals.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.LunchDeals;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetLunchDealsByRestaurantQuery(int RestaurantId) : IRequest<IReadOnlyList<LunchDealDto>>;

public sealed class GetLunchDealsByRestaurantQueryHandler
    : IRequestHandler<GetLunchDealsByRestaurantQuery, IReadOnlyList<LunchDealDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLunchDealsByRestaurantQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<LunchDealDto>> Handle(
        GetLunchDealsByRestaurantQuery request,
        CancellationToken cancellationToken)
    {
        var deals = await _context.LunchDeals
            .Where(d => d.RestaurantId == request.RestaurantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return deals.Adapt<List<LunchDealDto>>();
    }
}
