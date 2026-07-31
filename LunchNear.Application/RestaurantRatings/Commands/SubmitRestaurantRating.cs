namespace LunchNear.Application.RestaurantRatings.Commands;

using FluentValidation;
using global::Mediator;
using LunchNear.Application.Common.Exceptions;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Ratings;
using LunchNear.Domain.Entities;
using LunchNear.Domain.ValueObjects;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record SubmitRestaurantRatingCommand(int RestaurantId, string UserId, int Rating, string? Review)
    : IRequest<RestaurantRatingDto>;

public sealed class SubmitRestaurantRatingCommandValidator : AbstractValidator<SubmitRestaurantRatingCommand>
{
    public SubmitRestaurantRatingCommandValidator()
    {
        RuleFor(x => x.RestaurantId).GreaterThan(0);
        RuleFor(x => x.UserId).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Rating).InclusiveBetween(RatingValue.MinValue, RatingValue.MaxValue);
        RuleFor(x => x.Review).MaximumLength(1000);
    }
}

public sealed class SubmitRestaurantRatingCommandHandler : IRequestHandler<SubmitRestaurantRatingCommand, RestaurantRatingDto>
{
    private readonly IApplicationDbContext _context;

    public SubmitRestaurantRatingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<RestaurantRatingDto> Handle(SubmitRestaurantRatingCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Restaurant), request.RestaurantId);

        var rating = restaurant.AddRating(request.UserId, RatingValue.Create(request.Rating), request.Review);

        await _context.SaveChangesAsync(cancellationToken);

        return rating.Adapt<RestaurantRatingDto>();
    }
}
