namespace LunchNear.Contracts.Ratings;

public sealed record DishRatingDto(
    int Id,
    int DishId,
    string UserId,
    int Rating,
    string? Review,
    DateTimeOffset CreatedAt);
