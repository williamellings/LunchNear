namespace LunchNear.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using LunchNear.Shared.Models;
using LunchNear.Shared.Services;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantsController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Restaurant>>> GetAll()
    {
        var restaurants = await _restaurantService.GetAllRestaurants();
        return Ok(restaurants);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Restaurant>> GetById(int id)
    {
        var restaurant = await _restaurantService.GetRestaurantById(id);
        if (restaurant == null)
        {
            return NotFound();
        }

        return Ok(restaurant);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Restaurant>>> Search([FromQuery] string query)
    {
        var results = await _restaurantService.SearchRestaurants(query);
        return Ok(results);
    }

    [HttpPost("filter")]
    public async Task<ActionResult<IEnumerable<Restaurant>>> Filter([FromBody] RestaurantFilterCriteria criteria)
    {
        var results = await _restaurantService.FilterRestaurants(criteria);
        return Ok(results);
    }
}
