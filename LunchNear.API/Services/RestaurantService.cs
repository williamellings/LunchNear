namespace LunchNear.API.Services;

using LunchNear.Shared.Models;
using LunchNear.Shared.Services;

/// <summary>
/// Implementation of restaurant data service with mock data.
/// </summary>
public class RestaurantService : IRestaurantService
{
    private readonly List<Restaurant> _restaurants;

    public RestaurantService()
    {
        _restaurants = InitializeMockData();
    }

    public Task<IEnumerable<Restaurant>> GetAllRestaurants()
    {
        return Task.FromResult<IEnumerable<Restaurant>>(_restaurants);
    }

    public Task<Restaurant?> GetRestaurantById(int id)
    {
        var restaurant = _restaurants.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(restaurant);
    }

    public Task<IEnumerable<Restaurant>> SearchRestaurants(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult<IEnumerable<Restaurant>>(_restaurants);
        }

        var searchQuery = query.ToLower().Trim();
        var results = _restaurants
            .Where(r => r.Name.ToLower().Contains(searchQuery))
            .ToList();

        return Task.FromResult<IEnumerable<Restaurant>>(results);
    }

    public Task<IEnumerable<Restaurant>> FilterRestaurants(RestaurantFilterCriteria criteria)
    {
        var results = _restaurants.AsEnumerable();

        if (criteria == null ||
            (string.IsNullOrWhiteSpace(criteria.PriceRange) &&
             !criteria.HasStudentDiscount.HasValue &&
             !criteria.HasLunchBuffet.HasValue))
        {
            return Task.FromResult(results);
        }

        if (!string.IsNullOrWhiteSpace(criteria.PriceRange))
        {
            results = results.Where(r => r.PriceRange == criteria.PriceRange);
        }

        if (criteria.HasStudentDiscount.HasValue)
        {
            results = results.Where(r => r.HasStudentDiscount == criteria.HasStudentDiscount.Value);
        }

        if (criteria.HasLunchBuffet.HasValue)
        {
            results = results.Where(r => r.HasLunchBuffet == criteria.HasLunchBuffet.Value);
        }

        return Task.FromResult(results.ToList() as IEnumerable<Restaurant>);
    }

    private List<Restaurant> InitializeMockData()
    {
        return new List<Restaurant>
        {
            new()
            {
                Id = 1,
                Name = "Burger King",
                Address = "Avenyn, Gothenburg",
                Rating = 3.8m,
                PriceRange = "Cheap",
                HasLunchBuffet = false,
                HasStudentDiscount = true
            },
            new()
            {
                Id = 2,
                Name = "Pizzeria Marco",
                Address = "Magasinsgatan, Gothenburg",
                Rating = 4.2m,
                PriceRange = "Medium",
                HasLunchBuffet = true,
                HasStudentDiscount = true
            },
            new()
            {
                Id = 3,
                Name = "Sushi Paradise",
                Address = "Kungsgatan, Gothenburg",
                Rating = 4.5m,
                PriceRange = "Expensive",
                HasLunchBuffet = true,
                HasStudentDiscount = false
            },
            new()
            {
                Id = 4,
                Name = "Tacos El Amigo",
                Address = "Nils Ericson Plats, Gothenburg",
                Rating = 4.1m,
                PriceRange = "Cheap",
                HasLunchBuffet = false,
                HasStudentDiscount = true
            },
            new()
            {
                Id = 5,
                Name = "Thai Kitchen",
                Address = "Jarntorget, Gothenburg",
                Rating = 4.3m,
                PriceRange = "Medium",
                HasLunchBuffet = true,
                HasStudentDiscount = true
            },
            new()
            {
                Id = 6,
                Name = "Steakhouse Premium",
                Address = "Storgatan, Gothenburg",
                Rating = 4.7m,
                PriceRange = "Expensive",
                HasLunchBuffet = false,
                HasStudentDiscount = false
            },
            new()
            {
                Id = 7,
                Name = "Kebab House",
                Address = "Heden, Gothenburg",
                Rating = 3.9m,
                PriceRange = "Cheap",
                HasLunchBuffet = false,
                HasStudentDiscount = true
            },
            new()
            {
                Id = 8,
                Name = "Pasta Perfetto",
                Address = "Vasaplatsen, Gothenburg",
                Rating = 4.4m,
                PriceRange = "Medium",
                HasLunchBuffet = true,
                HasStudentDiscount = false
            }
        };
    }
}
