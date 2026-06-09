namespace LunchNear.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using LunchNear.Shared.Models;
using LunchNear.Shared.Services;

[ApiController]
[Route("api/dishes/{dishId}/[controller]")]
public class DishRatingsController : ControllerBase
{
    private readonly IDishRatingService _dishRatingService;

    public DishRatingsController(IDishRatingService dishRatingService)
    {
        _dishRatingService = dishRatingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DishRating>>> GetRatings(int dishId)
    {
        var ratings = await _dishRatingService.GetDishRatings(dishId);
        return Ok(ratings);
    }

    [HttpPost]
    public async Task<ActionResult> SubmitRating(int dishId, [FromBody] DishRating rating)
    {
        if (rating.DishId != dishId)
        {
            return BadRequest("DishId in URL and body must match");
        }

        await _dishRatingService.SubmitDishRating(rating);
        return CreatedAtAction(nameof(GetRatings), new { dishId }, rating);
    }

    [HttpGet("average")]
    public async Task<ActionResult<decimal>> GetAverageRating(int dishId)
    {
        var average = await _dishRatingService.CalculateAverageDishRating(dishId);
        return Ok(average);
    }
}
