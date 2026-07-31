namespace LunchNear.Contracts.StudentDiscounts;

public sealed record StudentDiscountDto(
    int Id,
    int RestaurantId,
    string Description,
    int DiscountPercentage);
