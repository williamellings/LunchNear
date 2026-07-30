namespace LunchNear.Shared.Models;

/// <summary>
/// Represents a dish/food item offered by a restaurant.
/// </summary>
public class Dish
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int RestaurantId { get; set; }
    public decimal Price { get; set; }

    /// <summary>
    /// Average rating of the dish (0-5 stars).
    /// Unique feature: Users can rate individual dishes, not just restaurants.
    /// </summary>
    public decimal Rating { get; set; }

    public string? Description { get; set; }

    public Restaurant? Restaurant { get; set; }
    public ICollection<DishRating> Ratings { get; set; } = new List<DishRating>();

}
