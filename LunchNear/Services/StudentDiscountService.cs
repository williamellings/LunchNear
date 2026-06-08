namespace LunchNear.Services;

using LunchNear.Models;

public class StudentDiscountService : IStudentDiscountService
{
    private readonly List<StudentDiscount> _discounts;

    public StudentDiscountService()
    {
        _discounts = InitializeMockData();
    }

    public Task<IEnumerable<StudentDiscount>> GetStudentDiscounts(int restaurantId)
    {
        var discounts = _discounts
            .Where(d => d.RestaurantId == restaurantId)
            .ToList();

        return Task.FromResult(discounts.AsEnumerable());
    }

    public Task<bool> HasStudentDiscount(int restaurantId)
    {
        var hasDiscount = _discounts.Any(d => d.RestaurantId == restaurantId);
        return Task.FromResult(hasDiscount);
    }

    public async Task<IEnumerable<Restaurant>> GetRestaurantsWithStudentDiscounts(IEnumerable<Restaurant> restaurants)
    {
        var restaurantsWithDiscounts = new List<Restaurant>();

        foreach (var restaurant in restaurants)
        {
            var hasDiscount = await HasStudentDiscount(restaurant.Id);
            if (hasDiscount)
            {
                restaurantsWithDiscounts.Add(restaurant);
            }
        }

        return restaurantsWithDiscounts;
    }

    private List<StudentDiscount> InitializeMockData()
    {
        return new List<StudentDiscount>
        {
            new()
            {
                Id = 1,
                RestaurantId = 1,
                Description = "20% off with valid student ID",
                DiscountPercentage = 20
            },
            new()
            {
                Id = 2,
                RestaurantId = 2,
                Description = "Buy one pizza get second 50% off",
                DiscountPercentage = 50
            },
            new()
            {
                Id = 3,
                RestaurantId = 4,
                Description = "15% student discount with ID",
                DiscountPercentage = 15
            },
            new()
            {
                Id = 4,
                RestaurantId = 5,
                Description = "10% off for students every day",
                DiscountPercentage = 10
            },
            new()
            {
                Id = 5,
                RestaurantId = 7,
                Description = "Free drink with student discount",
                DiscountPercentage = 5
            }
        };
    }
}
