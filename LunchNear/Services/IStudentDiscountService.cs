namespace LunchNear.Services;

using LunchNear.Models;

public interface IStudentDiscountService
{
    Task<IEnumerable<StudentDiscount>> GetStudentDiscounts(int restaurantId);
    Task<bool> HasStudentDiscount(int restaurantId);
    Task<IEnumerable<Restaurant>> GetRestaurantsWithStudentDiscounts(IEnumerable<Restaurant> restaurants);
}
