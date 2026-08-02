namespace LunchNear.Application.StudentDiscounts.Commands;

using FluentValidation;
using global::Mediator;
using LunchNear.Application.Common.Exceptions;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.StudentDiscounts;
using LunchNear.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record AddStudentDiscountCommand(int RestaurantId, string Description, int DiscountPercentage)
    : IRequest<StudentDiscountDto>;

public sealed class AddStudentDiscountCommandValidator : AbstractValidator<AddStudentDiscountCommand>
{
    public AddStudentDiscountCommandValidator()
    {
        RuleFor(x => x.RestaurantId).GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(300);
        RuleFor(x => x.DiscountPercentage).InclusiveBetween(1, 100);
    }
}

public sealed class AddStudentDiscountCommandHandler : IRequestHandler<AddStudentDiscountCommand, StudentDiscountDto>
{
    private readonly IApplicationDbContext _context;

    public AddStudentDiscountCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<StudentDiscountDto> Handle(AddStudentDiscountCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Restaurant), request.RestaurantId);

        var discount = restaurant.AddStudentDiscount(request.Description, request.DiscountPercentage);

        await _context.SaveChangesAsync(cancellationToken);

        return discount.Adapt<StudentDiscountDto>();
    }
}
