namespace LunchNear.Contracts.LunchDeals;

public sealed record CreateLunchDealRequest(
    string Name,
    string? Description,
    decimal Price,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string DaysOfWeek);
