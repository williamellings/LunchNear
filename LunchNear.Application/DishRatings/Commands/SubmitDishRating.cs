namespace LunchNear.Application.DishRatings.Commands;

using FluentValidation;
using global::Mediator;
using LunchNear.Application.Common.Exceptions;
using LunchNear.Application.Common.Interfaces;
using LunchNear.Contracts.Ratings;
using LunchNear.Domain.Entities;
using LunchNear.Domain.ValueObjects;
using Mapster;
using Microsoft.EntityFrameworkCore;

public sealed record SubmitDishRatingCommand(int DishId, string UserId, int Rating, string? Review)
    : IRequest<DishRatingDto>;

public sealed class SubmitDishRatingCommandValidator : AbstractValidator<SubmitDishRatingCommand>
{
    public SubmitDishRatingCommandValidator()
    {
        RuleFor(x => x.DishId).GreaterThan(0);
        RuleFor(x => x.UserId).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Rating).InclusiveBetween(RatingValue.MinValue, RatingValue.MaxValue);
        RuleFor(x => x.Review).MaximumLength(1000);
    }
}

/// <summary>
/// Loads only the Dish aggregate (never the parent Restaurant) so submitting a dish rating
/// can never accidentally touch, or be coupled to, the restaurant's own rating.
/// </summary>
public sealed class SubmitDishRatingCommandHandler : IRequestHandler<SubmitDishRatingCommand, DishRatingDto>
{
    private readonly IApplicationDbContext _context;

    public SubmitDishRatingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<DishRatingDto> Handle(SubmitDishRatingCommand request, CancellationToken cancellationToken)
    {
        var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == request.DishId, cancellationToken)
            ?? throw new NotFoundException(nameof(Dish), request.DishId);

        var rating = dish.AddRating(request.UserId, RatingValue.Create(request.Rating), request.Review);

        await _context.SaveChangesAsync(cancellationToken);

        return rating.Adapt<DishRatingDto>();
    }
}
