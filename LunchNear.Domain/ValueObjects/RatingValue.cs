namespace LunchNear.Domain.ValueObjects;

using LunchNear.Domain.Common;
using LunchNear.Domain.Exceptions;

/// <summary>
/// A self-validating 1-5 star rating. Used identically by both restaurant and dish ratings
/// so the 1-5 invariant can never be enforced in one place and forgotten in another.
/// </summary>
public sealed class RatingValue : ValueObject
{
    public const int MinValue = 1;
    public const int MaxValue = 5;

    public int Value { get; }

    private RatingValue(int value)
    {
        Value = value;
    }

    public static RatingValue Create(int value)
    {
        if (value < MinValue || value > MaxValue)
        {
            throw new DomainException($"Rating must be between {MinValue} and {MaxValue}, got {value}.");
        }

        return new RatingValue(value);
    }

    public static implicit operator int(RatingValue rating) => rating.Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
