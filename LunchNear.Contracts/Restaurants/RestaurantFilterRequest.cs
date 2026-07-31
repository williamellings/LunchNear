namespace LunchNear.Contracts.Restaurants;

public sealed record RestaurantFilterRequest(
    string? PriceRange = null,
    bool? HasStudentDiscount = null,
    bool? HasLunchDeals = null);
