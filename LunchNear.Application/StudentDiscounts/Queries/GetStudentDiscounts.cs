namespace LunchNear.Application.StudentDiscounts.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.StudentDiscounts;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record GetStudentDiscountsQuery(int RestaurantId) : IRequest<IReadOnlyList<StudentDiscountDto>>;

public sealed class GetStudentDiscountsQueryHandler : IRequestHandler<GetStudentDiscountsQuery, IReadOnlyList<StudentDiscountDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStudentDiscountsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IReadOnlyList<StudentDiscountDto>> Handle(
        GetStudentDiscountsQuery request,
        CancellationToken cancellationToken)
    {
        var discounts = await _context.StudentDiscounts
            .Where(d => d.RestaurantId == request.RestaurantId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return discounts.Adapt<List<StudentDiscountDto>>();
    }
}
