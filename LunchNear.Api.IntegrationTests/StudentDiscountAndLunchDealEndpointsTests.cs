namespace LunchNear.Api.IntegrationTests;

using System.Net.Http.Json;
using LunchNear.Contracts.LunchDeals;
using LunchNear.Contracts.Restaurants;
using LunchNear.Contracts.StudentDiscounts;
using LunchNear.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class StudentDiscountAndLunchDealEndpointsTests : IClassFixture<LunchNearApiFactory>
{
    private readonly HttpClient _client;
    private readonly LunchNearApiFactory _factory;

    public StudentDiscountAndLunchDealEndpointsTests(LunchNearApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStudentDiscounts_ForRestaurantWithDiscount_ReturnsDiscounts()
    {
        var restaurants = await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants");
        var restaurant = restaurants!.First();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var entity = await context.Restaurants.FirstAsync(r => r.Id == restaurant.Id);
            entity.AddStudentDiscount("15% off with student ID", 15);
            await context.SaveChangesAsync();
        }

        var discounts = await _client.GetFromJsonAsync<List<StudentDiscountDto>>(
            $"/api/restaurants/{restaurant.Id}/studentdiscounts");

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

    [Fact]
    public async Task AddStudentDiscount_ForRestaurant_PersistsDiscount()
    {
        var restaurants = await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants");
        var restaurant = restaurants!.First();

        var response = await _client.PostAsJsonAsync(
            $"/api/restaurants/{restaurant.Id}/studentdiscounts",
            new CreateStudentDiscountRequest("10% off with student ID", 10));

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<StudentDiscountDto>();

        Assert.NotNull(created);
        Assert.Equal(10, created!.DiscountPercentage);

        var discounts = await _client.GetFromJsonAsync<List<StudentDiscountDto>>(
            $"/api/restaurants/{restaurant.Id}/studentdiscounts");

        Assert.Contains(discounts!, d => d.Id == created.Id);
    }

    [Fact]
    public async Task AddLunchDeal_ForRestaurant_PersistsDeal()
    {
        var restaurants = await _client.GetFromJsonAsync<List<RestaurantDto>>("/api/restaurants");
        var restaurant = restaurants!.First();

        var response = await _client.PostAsJsonAsync(
            $"/api/restaurants/{restaurant.Id}/lunchdeals",
            new CreateLunchDealRequest(
                "Admin lunch",
                "Soup and main",
                109m,
                new TimeOnly(11, 0),
                new TimeOnly(14, 0),
                "Mon-Fri"));

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<LunchDealDto>();

        Assert.NotNull(created);
        Assert.Equal("Admin lunch", created!.Name);

        var deals = await _client.GetFromJsonAsync<List<LunchDealDto>>(
            $"/api/restaurants/{restaurant.Id}/lunchdeals");

        Assert.Contains(deals!, d => d.Id == created.Id);
    }
}
