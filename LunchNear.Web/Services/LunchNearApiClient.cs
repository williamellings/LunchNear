using System.Net.Http.Json;
using LunchNear.Contracts.Dishes;
using LunchNear.Contracts.LunchDeals;
using LunchNear.Contracts.Ratings;
using LunchNear.Contracts.Restaurants;
using LunchNear.Contracts.StudentDiscounts;

namespace LunchNear.Web.Services;

/// <summary>
/// Thin typed HTTP client wrapping the LunchNear.Api Minimal API. Speaks only in
/// LunchNear.Contracts DTOs - the browser bundle never references Domain or Application types.
/// </summary>
public class LunchNearApiClient
{
    private readonly HttpClient _http;

    public LunchNearApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<RestaurantDto>> GetRestaurantsAsync()
        => await _http.GetFromJsonAsync<List<RestaurantDto>>("api/restaurants") ?? [];

    public async Task<RestaurantDto?> GetRestaurantAsync(int id)
        => await _http.GetFromJsonAsync<RestaurantDto>($"api/restaurants/{id}");

    public async Task<List<RestaurantDto>> SearchRestaurantsAsync(string query)
        => await _http.GetFromJsonAsync<List<RestaurantDto>>($"api/restaurants/search?query={Uri.EscapeDataString(query)}") ?? [];

    public async Task<List<NearbyRestaurantDto>> GetNearbyRestaurantsAsync(double lat, double lng, double radiusKm = 5)
        => await _http.GetFromJsonAsync<List<NearbyRestaurantDto>>(
            $"api/restaurants/nearby?latitude={lat}&longitude={lng}&radiusKm={radiusKm}") ?? [];

    public async Task<List<RestaurantDto>> FilterRestaurantsAsync(RestaurantFilterRequest criteria)
    {
        var response = await _http.PostAsJsonAsync("api/restaurants/filter", criteria);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<RestaurantDto>>() ?? [];
    }

    public async Task<List<StudentDiscountDto>> GetStudentDiscountsAsync(int restaurantId)
        => await _http.GetFromJsonAsync<List<StudentDiscountDto>>($"api/restaurants/{restaurantId}/studentdiscounts") ?? [];

    public async Task<StudentDiscountDto> AddStudentDiscountAsync(int restaurantId, CreateStudentDiscountRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/restaurants/{restaurantId}/studentdiscounts", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StudentDiscountDto>())!;
    }

    public async Task<List<LunchDealDto>> GetLunchDealsAsync(int restaurantId)
        => await _http.GetFromJsonAsync<List<LunchDealDto>>($"api/restaurants/{restaurantId}/lunchdeals") ?? [];

    public async Task<LunchDealDto> AddLunchDealAsync(int restaurantId, CreateLunchDealRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/restaurants/{restaurantId}/lunchdeals", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<LunchDealDto>())!;
    }

    public async Task<List<DishDto>> GetDishesAsync(int restaurantId)
        => await _http.GetFromJsonAsync<List<DishDto>>($"api/restaurants/{restaurantId}/dishes") ?? [];

    public async Task<decimal> GetRestaurantAverageRatingAsync(int restaurantId)
        => await _http.GetFromJsonAsync<decimal>($"api/restaurants/{restaurantId}/ratings/average");

    public async Task<RestaurantRatingDto> SubmitRestaurantRatingAsync(int restaurantId, SubmitRatingRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/restaurants/{restaurantId}/ratings", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RestaurantRatingDto>())!;
    }

    public async Task<decimal> GetDishAverageRatingAsync(int dishId)
        => await _http.GetFromJsonAsync<decimal>($"api/dishes/{dishId}/ratings/average");

    public async Task<DishRatingDto> SubmitDishRatingAsync(int dishId, SubmitRatingRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/dishes/{dishId}/ratings", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DishRatingDto>())!;
    }
}
