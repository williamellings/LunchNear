namespace LunchNear.Application.LunchDeals.Commands;

using FluentValidation;
using global::Mediator;
using LunchNear.Application.Common.Exceptions;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.LunchDeals;
using LunchNear.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record AddLunchDealCommand(
    int RestaurantId,
    string Name,
    string? Description,
    decimal Price,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string DaysOfWeek) : IRequest<LunchDealDto>;

public sealed class AddLunchDealCommandValidator : AbstractValidator<AddLunchDealCommand>
{
    public AddLunchDealCommandValidator()
    {
        RuleFor(x => x.RestaurantId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DaysOfWeek).NotEmpty().MaximumLength(50);
        RuleFor(x => x).Must(x => x.EndTime > x.StartTime)
            .WithMessage("End time must be after start time.");
    }
}

public sealed class AddLunchDealCommandHandler : IRequestHandler<AddLunchDealCommand, LunchDealDto>
{
    private readonly IApplicationDbContext _context;

    public AddLunchDealCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<LunchDealDto> Handle(AddLunchDealCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Restaurant), request.RestaurantId);

        var deal = restaurant.AddLunchDeal(
            request.Name,
            request.Description,
            request.Price,
            request.StartTime,
            request.EndTime,
            request.DaysOfWeek);

        await _context.SaveChangesAsync(cancellationToken);

        return deal.Adapt<LunchDealDto>();
    }
}
