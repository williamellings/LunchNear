namespace LunchNear.Domain.Entities;

using LunchNear.Domain.Common;
using LunchNear.Domain.Enums;
using LunchNear.Domain.Exceptions;
using LunchNear.Domain.ValueObjects;

/// <summary>
/// Aggregate root for a restaurant. Owns its student discounts, lunch deals and its own
/// restaurant-level ratings. Deliberately does NOT own Dishes - dishes are their own aggregate
/// (see <see cref="Dish"/>) so a dish's rating is entirely independent of the restaurant's rating.
/// </summary>
public class Restaurant : BaseEntity
{
    private readonly List<StudentDiscount> _studentDiscounts = [];
    private readonly List<LunchDeal> _lunchDeals = [];
    private readonly List<RestaurantRating> _ratings = [];

    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public PriceRange PriceRange { get; private set; }
    public GeoLocation Location { get; private set; } = null!;

    /// <summary>Cached average of this restaurant's own ratings only - never mixed with dish ratings.</summary>
    public decimal AverageRating { get; private set; }
    public int RatingsCount { get; private set; }

    public IReadOnlyCollection<StudentDiscount> StudentDiscounts => _studentDiscounts.AsReadOnly();
    public IReadOnlyCollection<LunchDeal> LunchDeals => _lunchDeals.AsReadOnly();
    public IReadOnlyCollection<RestaurantRating> Ratings => _ratings.AsReadOnly();

    public bool HasStudentDiscount => _studentDiscounts.Count > 0;
    public bool HasLunchDeals => _lunchDeals.Count > 0;

    private Restaurant()
    {
    }

    public static Restaurant Create(string name, string address, PriceRange priceRange, GeoLocation location)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Restaurant name is required.");
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            throw new DomainException("Restaurant address is required.");
        }

        ArgumentNullException.ThrowIfNull(location);

        return new Restaurant
        {
            Name = name,
            Address = address,
            PriceRange = priceRange,
            Location = location
        };
    }

    /// <summary>
    /// Adds a rating and updates the cached average incrementally from the cached
    /// AverageRating/RatingsCount alone - it deliberately does NOT require the full
    /// <see cref="Ratings"/> history to be loaded, so rating a restaurant stays a cheap,
    /// single-row write no matter how many ratings the restaurant has already accumulated.
    /// </summary>
    public RestaurantRating AddRating(string userId, RatingValue rating, string? review)
    {
        var restaurantRating = RestaurantRating.Create(userId, rating, review);
        _ratings.Add(restaurantRating);

        var totalScore = (AverageRating * RatingsCount) + rating.Value;
        RatingsCount += 1;
        AverageRating = Math.Round(totalScore / RatingsCount, 2);

        return restaurantRating;
    }

    public StudentDiscount AddStudentDiscount(string description, int discountPercentage)
    {
        var discount = StudentDiscount.Create(description, discountPercentage);
        _studentDiscounts.Add(discount);
        return discount;
    }

    public LunchDeal AddLunchDeal(
        string name,
        string? description,
        decimal price,
        TimeOnly startTime,
        TimeOnly endTime,
        string daysOfWeek)
    {
        var deal = LunchDeal.Create(name, description, price, startTime, endTime, daysOfWeek);
        _lunchDeals.Add(deal);
        return deal;
    }
}
