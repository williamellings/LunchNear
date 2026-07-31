namespace LunchNear.Contracts.Ratings;

/// <summary>
/// Shared request shape for submitting either a restaurant rating or a dish rating -
/// the target entity id always comes from the route, never from the body.
/// </summary>
public sealed record SubmitRatingRequest(string UserId, int Rating, string? Review = null);
