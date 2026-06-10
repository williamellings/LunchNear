namespace LunchNear.API.Services;

using Microsoft.EntityFrameworkCore;
using LunchNear.API.Data;
using LunchNear.Shared.Models;
using LunchNear.Shared.Services;

public class RestaurantService : IRestaurantService
{
    private readonly LunchNearDbContext _dbContext;

    public RestaurantService(LunchNearDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Restaurant>> GetAllRestaurants()
    {
        return await _dbContext.Restaurants.ToListAsync();
    }

    public async Task<Restaurant?> GetRestaurantById(int id)
    {
        return await _dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Restaurant>> SearchRestaurants(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await _dbContext.Restaurants.ToListAsync();
        }

        var searchQuery = query.ToLower().Trim();
        return await _dbContext.Restaurants
            .Where(r => r.Name.ToLower().Contains(searchQuery))
            .ToListAsync();
    }

    public async Task<IEnumerable<Restaurant>> FilterRestaurants(RestaurantFilterCriteria criteria)
    {
        var query = _dbContext.Restaurants.AsQueryable();

        if (criteria == null ||
            (string.IsNullOrWhiteSpace(criteria.PriceRange) &&
             !criteria.HasStudentDiscount.HasValue &&
             !criteria.HasLunchBuffet.HasValue))
        {
            return await query.ToListAsync();
        }

        if (!string.IsNullOrWhiteSpace(criteria.PriceRange))
        {
            query = query.Where(r => r.PriceRange == criteria.PriceRange);
        }

        if (criteria.HasStudentDiscount.HasValue)
        {
            query = query.Where(r => r.HasStudentDiscount == criteria.HasStudentDiscount.Value);
        }

        if (criteria.HasLunchBuffet.HasValue)
        {
            query = query.Where(r => r.HasLunchBuffet == criteria.HasLunchBuffet.Value);
        }

        return await query.ToListAsync();
    }
}
