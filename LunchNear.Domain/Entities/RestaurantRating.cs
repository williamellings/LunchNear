namespace LunchNear.Domain.Entities;

using LunchNear.Domain.Common;
using LunchNear.Domain.Exceptions;
using LunchNear.Domain.ValueObjects;

/// <summary>
/// A single user's rating of a restaurant as a whole. Child entity of the Restaurant aggregate -
/// created only through Restaurant.AddRating so the cached average can never drift out of sync.
/// </summary>
public class RestaurantRating : BaseEntity, IAuditableEntity
{
    public int RestaurantId { get; private set; }
    public string UserId { get; private set; } = null!;
    public RatingValue Rating { get; private set; } = null!;
    public string? Review { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Restaurant? Restaurant { get; private set; }

    private RestaurantRating()
    {
    }

    internal static RestaurantRating Create(string userId, RatingValue rating, string? review)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("UserId is required to submit a restaurant rating.");
        }

        ArgumentNullException.ThrowIfNull(rating);

        return new RestaurantRating
        {
            UserId = userId,
            Rating = rating,
            Review = review
        };
    }
}
