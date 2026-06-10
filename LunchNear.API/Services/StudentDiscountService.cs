namespace LunchNear.API.Services;

using Microsoft.EntityFrameworkCore;
using LunchNear.API.Data;
using LunchNear.Shared.Models;
using LunchNear.Shared.Services;

public class StudentDiscountService : IStudentDiscountService
{
    private readonly LunchNearDbContext _dbContext;

    public StudentDiscountService(LunchNearDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<StudentDiscount>> GetStudentDiscounts(int restaurantId)
    {
        return await _dbContext.StudentDiscounts
            .Where(d => d.RestaurantId == restaurantId)
            .ToListAsync();
    }

    public async Task<bool> HasStudentDiscount(int restaurantId)
    {
        return await _dbContext.StudentDiscounts
            .AnyAsync(d => d.RestaurantId == restaurantId);
    }

    public async Task<IEnumerable<Restaurant>> GetRestaurantsWithStudentDiscounts(IEnumerable<Restaurant> restaurants)
    {
        var restaurantIds = restaurants.Select(r => r.Id).ToList();
        var restaurantsWithDiscounts = await _dbContext.StudentDiscounts
            .Where(d => restaurantIds.Contains(d.RestaurantId))
            .Select(d => d.RestaurantId)
            .Distinct()
            .ToListAsync();

        return restaurants.Where(r => restaurantsWithDiscounts.Contains(r.Id));
    }
}
