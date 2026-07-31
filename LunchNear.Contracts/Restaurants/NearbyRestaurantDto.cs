namespace LunchNear.Contracts.Restaurants;

public sealed record NearbyRestaurantDto(RestaurantDto Restaurant, double DistanceKm);
