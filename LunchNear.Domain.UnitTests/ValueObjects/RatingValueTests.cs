namespace LunchNear.Domain.UnitTests.ValueObjects;

using LunchNear.Domain.Exceptions;
using LunchNear.Domain.ValueObjects;

public class RatingValueTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Create_WithValueInRange_Succeeds(int value)
    {
        var rating = RatingValue.Create(value);

        Assert.Equal(value, rating.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    [InlineData(100)]
    public void Create_WithValueOutOfRange_ThrowsDomainException(int value)
    {
        Assert.Throws<DomainException>(() => RatingValue.Create(value));
    }

    [Fact]
    public void ImplicitConversion_ToInt_ReturnsUnderlyingValue()
    {
        var rating = RatingValue.Create(4);

        int asInt = rating;

        Assert.Equal(4, asInt);
    }

    [Fact]
    public void Equality_IsValueBased()
    {
        var a = RatingValue.Create(3);
        var b = RatingValue.Create(3);
        var c = RatingValue.Create(4);

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }
}
