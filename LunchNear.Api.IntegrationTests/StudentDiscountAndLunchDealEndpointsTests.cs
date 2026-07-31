namespace LunchNear.Api.IntegrationTests;

using System.Net.Http.Json;
using LunchNear.Contracts.LunchDeals;
using LunchNear.Contracts.Restaurants;
using LunchNear.Contracts.StudentDiscounts;

public class StudentDiscountAndLunchDealEndpointsTests : IClassFixture<LunchNearApiFactory>
{
    private readonly HttpClient _client;

    public StudentDiscountAndLunchDealEndpointsTests(LunchNearApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStudentDiscounts_ForRestaurantWithDiscount_ReturnsDiscounts()
    {
        var restaurants = await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants");
        var withDiscount = restaurants!.First(r => r.HasStudentDiscount);

        var discounts = await _client.GetFromJsonAsync<List<StudentDiscountDto>>(
            $"/api/restaurants/{withDiscount.Id}/studentdiscounts");

        Assert.NotEmpty(discounts!);
    }

    [Fact]
    public async Task HasStudentDiscount_ForRestaurantWithoutDiscount_ReturnsFalse()
    {
        var restaurants = await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants");
        var withoutDiscount = restaurants!.First(r => !r.HasStudentDiscount);

        var hasDiscount = await _client.GetFromJsonAsync<bool>(
            $"/api/restaurants/{withoutDiscount.Id}/studentdiscounts/exists");

        Assert.False(hasDiscount);
    }

    [Fact]
    public async Task GetLunchDealsByRestaurant_ForRestaurantWithDeals_ReturnsDeals()
    {
        var restaurants = await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants");
        var withDeals = restaurants!.First(r => r.HasLunchDeals);

        var deals = await _client.GetFromJsonAsync<List<LunchDealDto>>($"/api/restaurants/{withDeals.Id}/lunchdeals");

        Assert.NotEmpty(deals!);
    }

    [Fact]
    public async Task GetAllActiveLunchDeals_ReturnsDealsAcrossRestaurants()
    {
        var deals = await _client.GetFromJsonAsync<List<LunchDealDto>>("/api/lunchdeals");

        Assert.NotEmpty(deals!);
    }
}
