namespace LunchNear.Application.UnitTests.LunchDeals;

using LunchNear.Application.LunchDeals.Queries;
using LunchNear.Application.UnitTests.Common;
using LunchNear.Domain.Entities;
using LunchNear.Domain.Enums;
using LunchNear.Domain.ValueObjects;

public class LunchDealQueryHandlerTests
{
    [Fact]
    public async Task GetLunchDealsByRestaurant_ReturnsDealsForThatRestaurantOnly()
    {
        using var context = ApplicationDbContextFactory.Create();
        var restaurant1 = Restaurant.Create("R1", "Addr", PriceRange.Medium, GeoLocation.Create(0, 0));
        restaurant1.AddLunchDeal("Lunch 1", null, 99m, new TimeOnly(12, 0), new TimeOnly(16, 0), "Mon-Fri");
        var restaurant2 = Restaurant.Create("R2", "Addr", PriceRange.Medium, GeoLocation.Create(0, 0));
        restaurant2.AddLunchDeal("Lunch 2", null, 89m, new TimeOnly(11, 0), new TimeOnly(15, 0), "Mon-Sun");
        context.Restaurants.AddRange(restaurant1, restaurant2);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetLunchDealsByRestaurantQueryHandler(context);
        var result = await handler.Handle(new GetLunchDealsByRestaurantQuery(restaurant1.Id), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Lunch 1", result[0].Name);
    }

    [Fact]
    public async Task GetAllActiveLunchDeals_ReturnsDealsAcrossAllRestaurants()
    {
        using var context = ApplicationDbContextFactory.Create();
        var restaurant1 = Restaurant.Create("R1", "Addr", PriceRange.Medium, GeoLocation.Create(0, 0));
        restaurant1.AddLunchDeal("Lunch 1", null, 99m, new TimeOnly(12, 0), new TimeOnly(16, 0), "Mon-Fri");
        var restaurant2 = Restaurant.Create("R2", "Addr", PriceRange.Medium, GeoLocation.Create(0, 0));
        restaurant2.AddLunchDeal("Lunch 2", null, 89m, new TimeOnly(11, 0), new TimeOnly(15, 0), "Mon-Sun");
        context.Restaurants.AddRange(restaurant1, restaurant2);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetAllActiveLunchDealsQueryHandler(context);
        var result = await handler.Handle(new GetAllActiveLunchDealsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }
}
