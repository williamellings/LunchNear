namespace LunchNear.Domain.Entities;

using LunchNear.Domain.Common;
using LunchNear.Domain.Exceptions;
using LunchNear.Domain.ValueObjects;

/// <summary>
/// A single user's rating of one specific dish - never aggregated into the restaurant's rating.
/// Child entity of the Dish aggregate, created only through Dish.AddRating.
/// </summary>
public class DishRating : BaseEntity, IAuditableEntity
{
    public int DishId { get; private set; }
    public string UserId { get; private set; } = null!;
    public RatingValue Rating { get; private set; } = null!;
    public string? Review { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Dish? Dish { get; private set; }

    private DishRating()
    {
    }

    internal static DishRating Create(string userId, RatingValue rating, string? review)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("UserId is required to submit a dish rating.");
        }

        ArgumentNullException.ThrowIfNull(rating);

        return new DishRating
        {
            UserId = userId,
            Rating = rating,
            Review = review
        };
    }
}
