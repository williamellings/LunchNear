namespace LunchNear.Models;

/// <summary>
/// Represents a student discount offer at a restaurant.
/// </summary>
public class StudentDiscount
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public required string Description { get; set; }
    public int DiscountPercentage { get; set; }
}
