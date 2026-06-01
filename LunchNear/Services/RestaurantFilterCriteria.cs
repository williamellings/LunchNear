namespace LunchNear.Services;

public class RestaurantFilterCriteria
{
    public string? PriceRange { get; set; }
    public bool? HasStudentDiscount { get; set; }
    public bool? HasLunchBuffet { get; set; }
}
