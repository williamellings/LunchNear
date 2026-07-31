namespace LunchNear.Contracts.Dishes;

/// <summary>
/// A dish's rating (AverageRating/RatingsCount) is computed strictly from its own ratings -
/// it is never derived from, or mixed with, the parent restaurant's rating.
/// </summary>
public sealed record DishDto(
    int Id,
    int RestaurantId,
    string Name,
    string? Description,
    decimal Price,
    decimal AverageRating,
    int RatingsCount);
