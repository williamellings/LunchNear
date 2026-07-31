namespace LunchNear.Application.LunchDeals.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.LunchDeals;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetAllActiveLunchDealsQuery : IRequest<IReadOnlyList<LunchDealDto>>;

public sealed class GetAllActiveLunchDealsQueryHandler : IRequestHandler<GetAllActiveLunchDealsQuery, IReadOnlyList<LunchDealDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllActiveLunchDealsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<LunchDealDto>> Handle(GetAllActiveLunchDealsQuery request, CancellationToken cancellationToken)
    {
        var deals = await _context.LunchDeals
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return deals.Adapt<List<LunchDealDto>>();
    }
}
