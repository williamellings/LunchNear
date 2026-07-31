namespace LunchNear.Application.UnitTests.StudentDiscounts;

using LunchNear.Application.StudentDiscounts.Queries;
using LunchNear.Application.UnitTests.Common;
using LunchNear.Domain.Entities;
using LunchNear.Domain.Enums;
using LunchNear.Domain.ValueObjects;

public class StudentDiscountQueryHandlerTests
{
    [Fact]
    public async Task GetStudentDiscounts_ReturnsDiscountsForThatRestaurantOnly()
    {
        using var context = ApplicationDbContextFactory.Create();
        var restaurant1 = Restaurant.Create("R1", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        restaurant1.AddStudentDiscount("15% off", 15);
        var restaurant2 = Restaurant.Create("R2", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        restaurant2.AddStudentDiscount("20% off", 20);
        context.Restaurants.AddRange(restaurant1, restaurant2);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStudentDiscountsQueryHandler(context);
        var result = await handler.Handle(new GetStudentDiscountsQuery(restaurant1.Id), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(15, result[0].DiscountPercentage);
    }

    [Fact]
    public async Task HasStudentDiscount_ForRestaurantWithDiscount_ReturnsTrue()
    {
        using var context = ApplicationDbContextFactory.Create();
        var restaurant = Restaurant.Create("R1", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        restaurant.AddStudentDiscount("15% off", 15);
        context.Restaurants.Add(restaurant);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new HasStudentDiscountQueryHandler(context);
        var result = await handler.Handle(new HasStudentDiscountQuery(restaurant.Id), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task HasStudentDiscount_ForRestaurantWithoutDiscount_ReturnsFalse()
    {
        using var context = ApplicationDbContextFactory.Create();
        var restaurant = Restaurant.Create("R1", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        context.Restaurants.Add(restaurant);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new HasStudentDiscountQueryHandler(context);
        var result = await handler.Handle(new HasStudentDiscountQuery(restaurant.Id), CancellationToken.None);

        Assert.False(result);
    }
}
