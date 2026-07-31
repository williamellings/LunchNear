namespace LunchNear.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using LunchNear.Contracts.Dishes;
using LunchNear.Contracts.Ratings;
using LunchNear.Contracts.Restaurants;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Each fact targets a different restaurant/dish (seeded restaurants are indexed 0..5) so tests
/// that mutate rating state can run against the single shared <see cref="LunchNearApiFactory"/>
/// instance without interfering with each other.
/// </summary>
public class RatingEndpointsTests : IClassFixture<LunchNearApiFactory>
{
    private readonly HttpClient _client;

    public RatingEndpointsTests(LunchNearApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<List<RestaurantDto>> GetRestaurantsAsync()
        => (await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants"))!;

    [Fact]
    public async Task SubmitDishRating_WithValidRating_Returns201AndUpdatesAverage()
    {
        var restaurants = await GetRestaurantsAsync();
        var dishes = await _client.GetFromJsonAsync<List<DishDto>>($"/api/restaurants/{restaurants[0].Id}/dishes");
        var dish = dishes!.First();
        var averageBefore = dish.AverageRating;
        var countBefore = dish.RatingsCount;

        var response = await _client.PostAsJsonAsync(
            $"/api/dishes/{dish.Id}/ratings", new SubmitRatingRequest("integration-test-user", 3, "Decent"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<DishRatingDto>();
        Assert.NotNull(created);
        Assert.Equal(dish.Id, created!.DishId);
        Assert.Equal(3, created.Rating);

        var expectedAverage = Math.Round(((averageBefore * countBefore) + 3) / (countBefore + 1), 2);
        var averageAfter = await _client.GetFromJsonAsync<decimal>($"/api/dishes/{dish.Id}/ratings/average");
        Assert.Equal(expectedAverage, averageAfter);
    }

    [Fact]
    public async Task SubmitDishRating_WithRatingOutOfRange_Returns400WithValidationProblemDetails()
    {
        var restaurants = await GetRestaurantsAsync();
        var dishes = await _client.GetFromJsonAsync<List<DishDto>>($"/api/restaurants/{restaurants[1].Id}/dishes");
        var dish = dishes!.First();

        var response = await _client.PostAsJsonAsync(
            $"/api/dishes/{dish.Id}/ratings", new SubmitRatingRequest("integration-test-user", 9, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains(problem!.Errors, e => e.Key == nameof(SubmitRatingRequest.Rating));
    }

    [Fact]
    public async Task SubmitDishRating_ForNonExistentDish_Returns404()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/dishes/999999/ratings", new SubmitRatingRequest("integration-test-user", 5, null));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SubmitRestaurantRating_WithValidRating_Returns201AndUpdatesAverage()
    {
        var restaurants = await GetRestaurantsAsync();
        var restaurant = restaurants[2];
        var averageBefore = restaurant.AverageRating;
        var countBefore = restaurant.RatingsCount;

        var response = await _client.PostAsJsonAsync(
            $"/api/restaurants/{restaurant.Id}/ratings", new SubmitRatingRequest("integration-test-user", 2, "Meh"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<RestaurantRatingDto>();
        Assert.NotNull(created);
        Assert.Equal(restaurant.Id, created!.RestaurantId);

        var expectedAverage = Math.Round(((averageBefore * countBefore) + 2) / (countBefore + 1), 2);
        var averageAfter = await _client.GetFromJsonAsync<decimal>($"/api/restaurants/{restaurant.Id}/ratings/average");
        Assert.Equal(expectedAverage, averageAfter);
    }

    [Fact]
    public async Task SubmitRestaurantRating_ForNonExistentRestaurant_Returns404()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/restaurants/999999/ratings", new SubmitRatingRequest("integration-test-user", 5, null));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDishRatings_AfterSubmitting_IncludesTheNewRating()
    {
        var restaurants = await GetRestaurantsAsync();
        var dishes = await _client.GetFromJsonAsync<List<DishDto>>($"/api/restaurants/{restaurants[3].Id}/dishes");
        var dish = dishes!.Last();

        await _client.PostAsJsonAsync(
            $"/api/dishes/{dish.Id}/ratings", new SubmitRatingRequest("rating-list-user", 4, "Solid"));

        var ratings = await _client.GetFromJsonAsync<List<DishRatingDto>>($"/api/dishes/{dish.Id}/ratings");
        Assert.Contains(ratings!, r => r.UserId == "rating-list-user" && r.Rating == 4);
    }
}
