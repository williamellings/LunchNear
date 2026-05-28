namespace LunchNear.Models;

/// <summary>
/// Represents a restaurant in the application.
/// </summary>
public class Restaurant
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public decimal Rating { get; set; }
    public required string PriceRange { get; set; }
    public bool HasLunchBuffet { get; set; }
    public bool HasStudentDiscount { get; set; }
}
