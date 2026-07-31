namespace LunchNear.Domain.ValueObjects;

using LunchNear.Domain.Common;
using LunchNear.Domain.Exceptions;

/// <summary>
/// A validated latitude/longitude pair with distance calculation, replacing two loose
/// primitive doubles on Restaurant.
/// </summary>
public sealed class GeoLocation : ValueObject
{
    private const double EarthRadiusKm = 6371;

    public double Latitude { get; }
    public double Longitude { get; }

    private GeoLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static GeoLocation Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
        {
            throw new DomainException($"Latitude must be between -90 and 90 degrees, got {latitude}.");
        }

        if (longitude is < -180 or > 180)
        {
            throw new DomainException($"Longitude must be between -180 and 180 degrees, got {longitude}.");
        }

        return new GeoLocation(latitude, longitude);
    }

    public double DistanceToKm(GeoLocation other)
    {
        ArgumentNullException.ThrowIfNull(other);

        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);
        var a = (Math.Sin(dLat / 2) * Math.Sin(dLat / 2)) +
                (Math.Cos(DegreesToRadians(Latitude)) * Math.Cos(DegreesToRadians(other.Latitude)) *
                 Math.Sin(dLon / 2) * Math.Sin(dLon / 2));
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}
