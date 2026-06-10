namespace LunchNear.API.Services;

using Microsoft.EntityFrameworkCore;
using LunchNear.API.Data;
using LunchNear.Shared.Models;
using LunchNear.Shared.Services;

public class DishRatingService : IDishRatingService
{
    private readonly LunchNearDbContext _dbContext;

    public DishRatingService(LunchNearDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SubmitDishRating(DishRating rating)
    {
        rating.CreatedAt = DateTime.Now;
        _dbContext.DishRatings.Add(rating);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<DishRating>> GetDishRatings(int dishId)
    {
        return await _dbContext.DishRatings
            .Where(r => r.DishId == dishId)
            .ToListAsync();
    }

    public async Task<decimal> CalculateAverageDishRating(int dishId)
    {
        var dishRatings = await _dbContext.DishRatings
            .Where(r => r.DishId == dishId)
            .ToListAsync();

        if (!dishRatings.Any())
        {
            return 0m;
        }

        return (decimal)dishRatings.Average(r => r.Rating);
    }

    public async Task<IEnumerable<Dish>> GetTopRatedDishesByRestaurant(int restaurantId)
    {
        var dishes = await _dbContext.Dishes
            .Where(d => d.RestaurantId == restaurantId)
            .ToListAsync();

        foreach (var dish in dishes)
        {
            var avgRating = await CalculateAverageDishRating(dish.Id);
            dish.Rating = avgRating;
        }

        return dishes.OrderByDescending(d => d.Rating);
    }
}
