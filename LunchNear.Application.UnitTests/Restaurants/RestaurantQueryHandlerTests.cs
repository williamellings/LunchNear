namespace LunchNear.Application.UnitTests.Restaurants;

using LunchNear.Application.Restaurants.Queries;
using LunchNear.Application.UnitTests.Common;
using LunchNear.Domain.Entities;
using LunchNear.Domain.Enums;
using LunchNear.Domain.ValueObjects;

public class RestaurantQueryHandlerTests
{
    private static Restaurant NewRestaurant(string name, string address, PriceRange priceRange, double lat = 57.7, double lng = 11.9)
        => Restaurant.Create(name, address, priceRange, GeoLocation.Create(lat, lng));

    [Fact]
    public async Task GetRestaurants_ReturnsAllRestaurantsAsDtos()
    {
        using var context = ApplicationDbContextFactory.Create();
        context.Restaurants.AddRange(
            NewRestaurant("Burger King", "Avenyn, Gothenburg", PriceRange.Cheap),
            NewRestaurant("Pizzeria Roma", "Kungsgatan", PriceRange.Medium));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetRestaurantsQueryHandler(context);
        var result = await handler.Handle(new GetRestaurantsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Name == "Burger King");
    }

    [Fact]
    public async Task GetRestaurants_IncludesStudentDiscountAndLunchDealFlags()
    {
        using var context = ApplicationDbContextFactory.Create();
        var withDiscount = NewRestaurant("Discount Place", "Addr", PriceRange.Cheap);
        withDiscount.AddStudentDiscount("15% off", 15);
        context.Restaurants.Add(withDiscount);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetRestaurantsQueryHandler(context);
        var result = await handler.Handle(new GetRestaurantsQuery(), CancellationToken.None);

        Assert.True(result.Single().HasStudentDiscount);
        Assert.False(result.Single().HasLunchDeals);
    }

    [Fact]
    public async Task GetRestaurantById_ExistingId_ReturnsDto()
    {
        using var context = ApplicationDbContextFactory.Create();
        var restaurant = NewRestaurant("Burger King", "Avenyn, Gothenburg", PriceRange.Cheap);
        context.Restaurants.Add(restaurant);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetRestaurantByIdQueryHandler(context);
        var result = await handler.Handle(new GetRestaurantByIdQuery(restaurant.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Burger King", result!.Name);
        Assert.Equal("Avenyn, Gothenburg", result.Address);
    }

    [Fact]
    public async Task GetRestaurantById_NonExistentId_ReturnsNull()
    {
        using var context = ApplicationDbContextFactory.Create();

        var handler = new GetRestaurantByIdQueryHandler(context);
        var result = await handler.Handle(new GetRestaurantByIdQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task SearchRestaurants_FindsByPartialCaseInsensitiveName()
    {
        using var context = ApplicationDbContextFactory.Create();
        context.Restaurants.AddRange(
            NewRestaurant("Burger King", "Addr1", PriceRange.Cheap),
            NewRestaurant("Pizzeria Roma", "Addr2", PriceRange.Medium));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new SearchRestaurantsQueryHandler(context);
        var result = await handler.Handle(new SearchRestaurantsQuery("pizzeria"), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Pizzeria Roma", result[0].Name);
    }

    [Fact]
    public async Task SearchRestaurants_WithNullOrWhitespaceQuery_ReturnsAll()
    {
        using var context = ApplicationDbContextFactory.Create();
        context.Restaurants.AddRange(
            NewRestaurant("Burger King", "Addr1", PriceRange.Cheap),
            NewRestaurant("Pizzeria Roma", "Addr2", PriceRange.Medium));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new SearchRestaurantsQueryHandler(context);
        var result = await handler.Handle(new SearchRestaurantsQuery(null), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SearchRestaurants_NoMatches_ReturnsEmpty()
    {
        using var context = ApplicationDbContextFactory.Create();
        context.Restaurants.Add(NewRestaurant("Burger King", "Addr1", PriceRange.Cheap));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new SearchRestaurantsQueryHandler(context);
        var result = await handler.Handle(new SearchRestaurantsQuery("NonExistent"), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetNearbyRestaurants_ReturnsOnlyWithinRadius_SortedByDistance()
    {
        using var context = ApplicationDbContextFactory.Create();
        var origin = GeoLocation.Create(57.7089, 11.9746);
        var near = Restaurant.Create("Near", "Addr", PriceRange.Cheap, GeoLocation.Create(57.7090, 11.9747));
        var far = Restaurant.Create("Far", "Addr", PriceRange.Cheap, GeoLocation.Create(59.3293, 18.0686)); // Stockholm
        context.Restaurants.AddRange(near, far);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetNearbyRestaurantsQueryHandler(context);
        var result = await handler.Handle(
            new GetNearbyRestaurantsQuery(origin.Latitude, origin.Longitude, RadiusKm: 10),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Near", result[0].Restaurant.Name);
    }
}
