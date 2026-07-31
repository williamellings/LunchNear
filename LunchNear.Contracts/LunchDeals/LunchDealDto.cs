namespace LunchNear.Contracts.LunchDeals;

public sealed record LunchDealDto(
    int Id,
    int RestaurantId,
    string Name,
    string? Description,
    decimal Price,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string DaysOfWeek);
