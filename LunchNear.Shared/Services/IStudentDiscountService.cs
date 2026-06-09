namespace LunchNear.Shared.Services;

using LunchNear.Shared.Models;

public interface IStudentDiscountService
{
    Task<IEnumerable<StudentDiscount>> GetStudentDiscounts(int restaurantId);
    Task<bool> HasStudentDiscount(int restaurantId);
    Task<IEnumerable<Restaurant>> GetRestaurantsWithStudentDiscounts(IEnumerable<Restaurant> restaurants);
}
