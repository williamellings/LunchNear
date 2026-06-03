namespace LunchNear.Services;

using LunchNear.Models;

public interface IDishRatingService
{
    Task SubmitDishRating(DishRating rating);
    Task<IEnumerable<DishRating>> GetDishRatings(int dishId);
    Task<decimal> CalculateAverageDishRating(int dishId);
    Task<IEnumerable<Dish>> GetTopRatedDishesByRestaurant(int restaurantId);
}
