namespace LunchNear.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using LunchNear.Shared.Models;
using LunchNear.Shared.Services;

[ApiController]
[Route("api/restaurants/{restaurantId}/[controller]")]
public class StudentDiscountsController : ControllerBase
{
    private readonly IStudentDiscountService _discountService;

    public StudentDiscountsController(IStudentDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDiscount>>> GetDiscounts(int restaurantId)
    {
        var discounts = await _discountService.GetStudentDiscounts(restaurantId);
        return Ok(discounts);
    }

    [HttpGet("has")]
    public async Task<ActionResult<bool>> HasDiscount(int restaurantId)
    {
        var hasDiscount = await _discountService.HasStudentDiscount(restaurantId);
        return Ok(hasDiscount);
    }
}
