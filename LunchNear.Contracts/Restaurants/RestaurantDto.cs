namespace LunchNear.Contracts.Restaurants;

/// <summary>
/// Flat, client-facing shape for a restaurant. Deliberately has no navigation collections
/// (no Dishes/Ratings/Discounts arrays) so the client never over-fetches or risks
/// serializing circular EF Core navigation graphs.
/// </summary>
public sealed record RestaurantDto(
    int Id,
    string Name,
    string Address,
    string PriceRange,
    double Latitude,
    double Longitude,
    decimal AverageRating,
    int RatingsCount,
    bool HasStudentDiscount,
    bool HasLunchDeals);
