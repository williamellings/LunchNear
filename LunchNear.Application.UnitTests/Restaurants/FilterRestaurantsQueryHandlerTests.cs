namespace LunchNear.Application.UnitTests.Restaurants;

using LunchNear.Application.Restaurants.Queries;
using LunchNear.Application.UnitTests.Common;
using LunchNear.Contracts.Restaurants;
using LunchNear.Domain.Entities;
using LunchNear.Domain.Enums;
using LunchNear.Domain.ValueObjects;

public class FilterRestaurantsQueryHandlerTests
{
    private static async Task<LunchNear.Infrastructure.Persistence.ApplicationDbContext> SeedAsync()
    {
        var context = ApplicationDbContextFactory.Create();

        var cheapWithDiscount = Restaurant.Create("Cheap+Discount", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        cheapWithDiscount.AddStudentDiscount("10% off", 10);

        var cheapNoDiscount = Restaurant.Create("Cheap Only", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));

        var mediumWithLunch = Restaurant.Create("Medium+Lunch", "Addr", PriceRange.Medium, GeoLocation.Create(0, 0));
        mediumWithLunch.AddLunchDeal("Lunch", null, 99m, new TimeOnly(12, 0), new TimeOnly(16, 0), "Mon-Fri");

        var expensive = Restaurant.Create("Expensive", "Addr", PriceRange.Expensive, GeoLocation.Create(0, 0));

        context.Restaurants.AddRange(cheapWithDiscount, cheapNoDiscount, mediumWithLunch, expensive);
        await context.SaveChangesAsync(CancellationToken.None);

        return context;
    }

    [Fact]
    public async Task Filter_WithNoCriteria_ReturnsAllRestaurants()
    {
        using var context = await SeedAsync();
        var handler = new FilterRestaurantsQueryHandler(context);

        var result = await handler.Handle(
            new FilterRestaurantsQuery(new RestaurantFilterRequest()), CancellationToken.None);

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public async Task Filter_ByPriceRange_ReturnsOnlyMatching()
    {
        using var context = await SeedAsync();
        var handler = new FilterRestaurantsQueryHandler(context);

        var result = await handler.Handle(
            new FilterRestaurantsQuery(new RestaurantFilterRequest(PriceRange: "Cheap")), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal("Cheap", r.PriceRange));
    }

    [Fact]
    public async Task Filter_ByHasStudentDiscountTrue_ReturnsOnlyWithDiscount()
    {
        using var context = await SeedAsync();
        var handler = new FilterRestaurantsQueryHandler(context);

        var result = await handler.Handle(
            new FilterRestaurantsQuery(new RestaurantFilterRequest(HasStudentDiscount: true)), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Cheap+Discount", result[0].Name);
    }

    [Fact]
    public async Task Filter_ByHasLunchDealsTrue_ReturnsOnlyWithLunchDeals()
    {
        using var context = await SeedAsync();
        var handler = new FilterRestaurantsQueryHandler(context);

        var result = await handler.Handle(
            new FilterRestaurantsQuery(new RestaurantFilterRequest(HasLunchDeals: true)), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Medium+Lunch", result[0].Name);
    }

    [Fact]
    public async Task Filter_ByCombinedCriteria_ReturnsMatchingIntersection()
    {
        using var context = await SeedAsync();
        var handler = new FilterRestaurantsQueryHandler(context);

        var result = await handler.Handle(
            new FilterRestaurantsQuery(new RestaurantFilterRequest(PriceRange: "Cheap", HasStudentDiscount: true)),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Cheap+Discount", result[0].Name);
    }

    [Fact]
    public async Task Filter_ByInvalidPriceRange_IsIgnored_ReturnsAll()
    {
        using var context = await SeedAsync();
        var handler = new FilterRestaurantsQueryHandler(context);

        var result = await handler.Handle(
            new FilterRestaurantsQuery(new RestaurantFilterRequest(PriceRange: "NotARealPrice")), CancellationToken.None);

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public async Task Filter_ByHasStudentDiscountFalse_ReturnsWithoutDiscount()
    {
        using var context = await SeedAsync();
        var handler = new FilterRestaurantsQueryHandler(context);

        var result = await handler.Handle(
            new FilterRestaurantsQuery(new RestaurantFilterRequest(HasStudentDiscount: false)), CancellationToken.None);

        Assert.Equal(3, result.Count);
        Assert.All(result, r => Assert.False(r.HasStudentDiscount));
    }
}
