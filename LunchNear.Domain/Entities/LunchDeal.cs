namespace LunchNear.Domain.Entities;

using LunchNear.Domain.Common;
using LunchNear.Domain.Exceptions;

public class LunchDeal : BaseEntity
{
    public int RestaurantId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public string DaysOfWeek { get; private set; } = null!;

    public Restaurant? Restaurant { get; private set; }

    private LunchDeal()
    {
    }

    internal static LunchDeal Create(
        string name,
        string? description,
        decimal price,
        TimeOnly startTime,
        TimeOnly endTime,
        string daysOfWeek)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Lunch deal name is required.");
        }

        if (price < 0)
        {
            throw new DomainException("Lunch deal price cannot be negative.");
        }

        if (endTime <= startTime)
        {
            throw new DomainException("Lunch deal end time must be after its start time.");
        }

        if (string.IsNullOrWhiteSpace(daysOfWeek))
        {
            throw new DomainException("Lunch deal days of week is required.");
        }

        return new LunchDeal
        {
            Name = name,
            Description = description,
            Price = price,
            StartTime = startTime,
            EndTime = endTime,
            DaysOfWeek = daysOfWeek
        };
    }
}
