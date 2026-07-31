namespace LunchNear.Contracts.Ratings;

public sealed record RestaurantRatingDto(
    int Id,
    int RestaurantId,
    string UserId,
    int Rating,
    string? Review,
    DateTimeOffset CreatedAt);
