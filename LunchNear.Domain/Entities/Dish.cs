namespace LunchNear.Domain.Entities;

using LunchNear.Domain.Common;
using LunchNear.Domain.Exceptions;
using LunchNear.Domain.ValueObjects;

/// <summary>
/// Aggregate root for a single dish. This is deliberately a SEPARATE aggregate from
/// <see cref="Restaurant"/> (even though it belongs to one via <see cref="RestaurantId"/>):
/// every dish carries its own independent rating, computed only from its own
/// <see cref="Ratings"/>, so rating a dish never affects - and is never affected by -
/// the parent restaurant's own rating.
/// </summary>
public class Dish : BaseEntity
{
    private readonly List<DishRating> _ratings = [];

    public int RestaurantId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }

    /// <summary>Cached average of this dish's own ratings only.</summary>
    public decimal AverageRating { get; private set; }
    public int RatingsCount { get; private set; }

    public Restaurant? Restaurant { get; private set; }
    public IReadOnlyCollection<DishRating> Ratings => _ratings.AsReadOnly();

    private Dish()
    {
    }

    public static Dish Create(int restaurantId, string name, string? description, decimal price)
    {
        if (restaurantId <= 0)
        {
            throw new DomainException("Dish must belong to a valid restaurant.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Dish name is required.");
        }

        if (price < 0)
        {
            throw new DomainException("Dish price cannot be negative.");
        }

        return new Dish
        {
            RestaurantId = restaurantId,
            Name = name,
            Description = description,
            Price = price
        };
    }

    /// <summary>
    /// Adds a rating and updates the cached average incrementally from the cached
    /// AverageRating/RatingsCount alone - it deliberately does NOT require the full
    /// <see cref="Ratings"/> history to be loaded, so rating a dish stays a cheap, single-row
    /// write no matter how many ratings the dish has already accumulated.
    /// </summary>
    public DishRating AddRating(string userId, RatingValue rating, string? review)
    {
        var dishRating = DishRating.Create(userId, rating, review);
        _ratings.Add(dishRating);

        var totalScore = (AverageRating * RatingsCount) + rating.Value;
        RatingsCount += 1;
        AverageRating = Math.Round(totalScore / RatingsCount, 2);

        return dishRating;
    }
}
