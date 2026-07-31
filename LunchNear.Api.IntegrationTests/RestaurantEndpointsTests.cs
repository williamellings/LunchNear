namespace LunchNear.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using LunchNear.Contracts.Restaurants;

public class RestaurantEndpointsTests : IClassFixture<LunchNearApiFactory>
{
    private readonly HttpClient _client;

    public RestaurantEndpointsTests(LunchNearApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRestaurants_ReturnsSeededRestaurants()
    {
        var response = await _client.GetAsync("/api/restaurants");

        response.EnsureSuccessStatusCode();
        var restaurants = await response.Content.ReadFromJsonAsync<List<RestaurantDto>>();

        Assert.NotNull(restaurants);
        Assert.NotEmpty(restaurants);
    }

    [Fact]
    public async Task GetRestaurantById_ExistingId_ReturnsRestaurant()
    {
        var restaurants = await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants");
        var firstId = restaurants!.First().Id;

        var response = await _client.GetAsync($"/api/restaurants/{firstId}");

        response.EnsureSuccessStatusCode();
        var restaurant = await response.Content.ReadFromJsonAsync<RestaurantDto>();
        Assert.NotNull(restaurant);
        Assert.Equal(firstId, restaurant!.Id);
    }

    [Fact]
    public async Task GetRestaurantById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/restaurants/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SearchRestaurants_WithMatchingQuery_ReturnsResults()
    {
        var all = await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants");
        var target = all!.First();
        var partialName = target.Name[..Math.Min(4, target.Name.Length)];

        var response = await _client.GetAsync($"/api/restaurants/search?query={Uri.EscapeDataString(partialName)}");

        response.EnsureSuccessStatusCode();
        var results = await response.Content.ReadFromJsonAsync<List<RestaurantDto>>();
        Assert.Contains(results!, r => r.Id == target.Id);
    }

    [Fact]
    public async Task FilterRestaurants_ByPriceRange_ReturnsOnlyMatching()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/restaurants/filter", new RestaurantFilterRequest(PriceRange: "Cheap"));

        response.EnsureSuccessStatusCode();
        var results = await response.Content.ReadFromJsonAsync<List<RestaurantDto>>();
        Assert.NotEmpty(results!);
        Assert.All(results!, r => Assert.Equal("Cheap", r.PriceRange));
    }

    [Fact]
    public async Task GetNearbyRestaurants_ReturnsRestaurantsWithDistance()
    {
        var response = await _client.GetAsync("/api/restaurants/nearby?latitude=57.7008&longitude=11.9765&radiusKm=50");

        response.EnsureSuccessStatusCode();
        var results = await response.Content.ReadFromJsonAsync<List<NearbyRestaurantDto>>();
        Assert.NotEmpty(results!);
        Assert.All(results!, r => Assert.True(r.DistanceKm <= 50));
    }
}
