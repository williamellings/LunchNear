namespace LunchNear.Contracts.StudentDiscounts;

public sealed record CreateStudentDiscountRequest(string Description, int DiscountPercentage);
