namespace LunchNear.Services;

using LunchNear.Models;

public interface IRestaurantService
{
    Task<IEnumerable<Restaurant>> GetAllRestaurants();
    Task<Restaurant?> GetRestaurantById(int id);
    Task<IEnumerable<Restaurant>> SearchRestaurants(string query);
    Task<IEnumerable<Restaurant>> FilterRestaurants(RestaurantFilterCriteria criteria);
}
