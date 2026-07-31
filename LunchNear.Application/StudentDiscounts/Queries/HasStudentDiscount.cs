namespace LunchNear.Application.StudentDiscounts.Queries;

using global::Mediator;
using LunchNear.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

public sealed record HasStudentDiscountQuery(int RestaurantId) : IRequest<bool>;

public sealed class HasStudentDiscountQueryHandler : IRequestHandler<HasStudentDiscountQuery, bool>
{
    private readonly IApplicationDbContext _context;

    public HasStudentDiscountQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public ValueTask<bool> Handle(HasStudentDiscountQuery request, CancellationToken cancellationToken)
    {
        return new ValueTask<bool>(
            _context.StudentDiscounts.AnyAsync(d => d.RestaurantId == request.RestaurantId, cancellationToken));
    }
}
