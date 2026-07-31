namespace LunchNear.Domain.Entities;

using LunchNear.Domain.Common;
using LunchNear.Domain.Exceptions;

public class StudentDiscount : BaseEntity
{
    public int RestaurantId { get; private set; }
    public string Description { get; private set; } = null!;
    public int DiscountPercentage { get; private set; }

    public Restaurant? Restaurant { get; private set; }

    private StudentDiscount()
    {
    }

    internal static StudentDiscount Create(string description, int discountPercentage)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Student discount description is required.");
        }

        if (discountPercentage is < 1 or > 100)
        {
            throw new DomainException($"Discount percentage must be between 1 and 100, got {discountPercentage}.");
        }

        return new StudentDiscount
        {
            Description = description,
            DiscountPercentage = discountPercentage
        };
    }
}
