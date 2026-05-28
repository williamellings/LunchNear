namespace LunchNear.Models;

/// <summary>
/// Represents a user rating for a specific dish.
/// </summary>
public class DishRating
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public required string UserId { get; set; }
    public int Rating { get; set; }
    public string? Review { get; set; }
    public DateTime CreatedAt { get; set; }
}
