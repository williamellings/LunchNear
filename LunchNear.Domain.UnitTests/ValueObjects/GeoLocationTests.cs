namespace LunchNear.Domain.UnitTests.ValueObjects;

using LunchNear.Domain.Exceptions;
using LunchNear.Domain.ValueObjects;

public class GeoLocationTests
{
    [Theory]
    [InlineData(-91, 0)]
    [InlineData(91, 0)]
    [InlineData(0, -181)]
    [InlineData(0, 181)]
    public void Create_WithOutOfRangeCoordinates_ThrowsDomainException(double latitude, double longitude)
    {
        Assert.Throws<DomainException>(() => GeoLocation.Create(latitude, longitude));
    }

    [Fact]
    public void Create_WithBoundaryCoordinates_Succeeds()
    {
        var location = GeoLocation.Create(90, 180);

        Assert.Equal(90, location.Latitude);
        Assert.Equal(180, location.Longitude);
    }

    [Fact]
    public void DistanceToKm_ToSelf_IsZero()
    {
        var location = GeoLocation.Create(57.7089, 11.9746);

        var distance = location.DistanceToKm(location);

        Assert.Equal(0, distance, precision: 6);
    }

    [Fact]
    public void DistanceToKm_BetweenKnownPoints_IsApproximatelyCorrect()
    {
        // Gothenburg central station to Liseberg amusement park - roughly 2.3 km apart.
        var gothenburgCentral = GeoLocation.Create(57.7089, 11.9746);
        var liseberg = GeoLocation.Create(57.6969, 11.9865);

        var distance = gothenburgCentral.DistanceToKm(liseberg);

        Assert.InRange(distance, 1.0, 3.0);
    }

    [Fact]
    public void Equality_IsValueBased()
    {
        var a = GeoLocation.Create(57.7, 11.9);
        var b = GeoLocation.Create(57.7, 11.9);
        var c = GeoLocation.Create(1, 1);

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }
}
