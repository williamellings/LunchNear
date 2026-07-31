namespace LunchNear.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using LunchNear.Contracts.Dishes;
using LunchNear.Contracts.Restaurants;

public class DishEndpointsTests : IClassFixture<LunchNearApiFactory>
{
    private readonly HttpClient _client;

    public DishEndpointsTests(LunchNearApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> GetAnyRestaurantIdAsync()
    {
        var restaurants = await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants");
        return restaurants!.First().Id;
    }

    [Fact]
    public async Task GetDishesByRestaurant_ReturnsDishesForThatRestaurant()
    {
        var restaurantId = await GetAnyRestaurantIdAsync();

        var response = await _client.GetAsync($"/api/restaurants/{restaurantId}/dishes");

        response.EnsureSuccessStatusCode();
        var dishes = await response.Content.ReadFromJsonAsync<List<DishDto>>();
        Assert.NotEmpty(dishes!);
        Assert.All(dishes!, d => Assert.Equal(restaurantId, d.RestaurantId));
    }

    [Fact]
    public async Task GetDishById_ExistingDish_ReturnsDish()
    {
        var restaurantId = await GetAnyRestaurantIdAsync();
        var dishes = await _client.GetFromJsonAsync<List<DishDto>>($"/api/restaurants/{restaurantId}/dishes");
        var dishId = dishes!.First().Id;

        var response = await _client.GetAsync($"/api/dishes/{dishId}");

        response.EnsureSuccessStatusCode();
        var dish = await response.Content.ReadFromJsonAsync<DishDto>();
        Assert.Equal(dishId, dish!.Id);
    }

    [Fact]
    public async Task GetDishById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/dishes/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTopRatedDishes_ReturnsOnlyDishesWithRatings()
    {
        var restaurantId = await GetAnyRestaurantIdAsync();

        var response = await _client.GetAsync($"/api/restaurants/{restaurantId}/dishes/top-rated");

        response.EnsureSuccessStatusCode();
        var dishes = await response.Content.ReadFromJsonAsync<List<DishDto>>();
        Assert.All(dishes!, d => Assert.True(d.RatingsCount > 0));
    }
}
