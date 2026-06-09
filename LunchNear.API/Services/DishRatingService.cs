namespace LunchNear.API.Services;

using LunchNear.Shared.Models;
using LunchNear.Shared.Services;

public class DishRatingService : IDishRatingService
{
    private readonly List<DishRating> _ratings = new();

    public Task SubmitDishRating(DishRating rating)
    {
        rating.Id = _ratings.Count > 0 ? _ratings.Max(r => r.Id) + 1 : 1;
        rating.CreatedAt = DateTime.Now;
        _ratings.Add(rating);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<DishRating>> GetDishRatings(int dishId)
    {
        var dishRatings = _ratings.Where(r => r.DishId == dishId).ToList();
        return Task.FromResult(dishRatings.AsEnumerable());
    }

    public Task<decimal> CalculateAverageDishRating(int dishId)
    {
        var dishRatings = _ratings.Where(r => r.DishId == dishId).ToList();

        if (!dishRatings.Any())
        {
            return Task.FromResult(0m);
        }

        var average = (decimal)dishRatings.Average(r => r.Rating);
        return Task.FromResult(average);
    }

    public async Task<IEnumerable<Dish>> GetTopRatedDishesByRestaurant(int restaurantId)
    {
        var topDishes = new List<Dish>();
        var mockDishes = GetMockDishesByRestaurant(restaurantId);

        foreach (var dish in mockDishes)
        {
            var avgRating = await CalculateAverageDishRating(dish.Id);
            dish.Rating = avgRating;
            topDishes.Add(dish);
        }

        return topDishes.OrderByDescending(d => d.Rating);
    }

    private List<Dish> GetMockDishesByRestaurant(int restaurantId)
    {
        return restaurantId switch
        {
            1 => new List<Dish>
            {
                new() { Id = 1, Name = "Whopper", RestaurantId = 1, Price = 8.99m, Description = "Classic flame-grilled burger" },
                new() { Id = 2, Name = "Chicken Sandwich", RestaurantId = 1, Price = 7.99m, Description = "Crispy chicken sandwich" }
            },
            2 => new List<Dish>
            {
                new() { Id = 3, Name = "Margherita Pizza", RestaurantId = 2, Price = 12.99m, Description = "Classic Italian pizza" },
                new() { Id = 4, Name = "Quattro Formaggi", RestaurantId = 2, Price = 14.99m, Description = "Four cheese pizza" }
            },
            3 => new List<Dish>
            {
                new() { Id = 5, Name = "Salmon Roll", RestaurantId = 3, Price = 15.99m, Description = "Fresh salmon sushi roll" },
                new() { Id = 6, Name = "Dragon Roll", RestaurantId = 3, Price = 16.99m, Description = "Tempura shrimp and avocado" }
            },
            _ => new List<Dish>()
        };
    }
}
