namespace LunchNear.Shared.Services;

using LunchNear.Shared.Models;

public interface IRestaurantService
{
    Task<IEnumerable<Restaurant>> GetAllRestaurants();
    Task<Restaurant?> GetRestaurantById(int id);
    Task<IEnumerable<Restaurant>> SearchRestaurants(string query);
    Task<IEnumerable<Restaurant>> FilterRestaurants(RestaurantFilterCriteria criteria);
}
