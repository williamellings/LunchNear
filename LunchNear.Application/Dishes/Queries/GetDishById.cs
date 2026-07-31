namespace LunchNear.Application.Dishes.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Dishes;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetDishByIdQuery(int DishId) : IRequest<DishDto?>;

public sealed class GetDishByIdQueryHandler : IRequestHandler<GetDishByIdQuery, DishDto?>
{
    private readonly IApplicationDbContext _context;

    public GetDishByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<DishDto?> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        var dish = await _context.Dishes
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.DishId, cancellationToken);

        return dish?.Adapt<DishDto>();
    }
}
