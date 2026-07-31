namespace LunchNear.Application.UnitTests.RestaurantRatings;

using LunchNear.Application.Common.Exceptions;
using LunchNear.Application.RestaurantRatings.Commands;
using LunchNear.Application.RestaurantRatings.Queries;
using LunchNear.Application.UnitTests.Common;
using LunchNear.Domain.Entities;
using LunchNear.Domain.Enums;
using LunchNear.Domain.ValueObjects;

public class RestaurantRatingHandlerTests
{
    [Fact]
    public async Task SubmitRestaurantRating_ForExistingRestaurant_PersistsRatingAndReturnsDto()
    {
        using var context = ApplicationDbContextFactory.Create();
        var restaurant = Restaurant.Create("Burger King", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        context.Restaurants.Add(restaurant);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new SubmitRestaurantRatingCommandHandler(context);
        var result = await handler.Handle(
            new SubmitRestaurantRatingCommand(restaurant.Id, "user1", 4, "Neplokho"), CancellationToken.None);

        Assert.Equal(restaurant.Id, result.RestaurantId);
        Assert.Equal(4, result.Rating);
        Assert.Equal("Neplokho", result.Review);
    }

    [Fact]
    public async Task SubmitRestaurantRating_ForNonExistentRestaurant_ThrowsNotFoundException()
    {
        using var context = ApplicationDbContextFactory.Create();
        var handler = new SubmitRestaurantRatingCommandHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new SubmitRestaurantRatingCommand(999, "user1", 4, null), CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task SubmitRestaurantRating_NeverAffectsAnyDishRating()
    {
        // Restaurant and dish ratings must stay fully independent aggregates.
        using var context = ApplicationDbContextFactory.Create();
        var restaurant = Restaurant.Create("Burger King", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        context.Restaurants.Add(restaurant);
        var dish = Dish.Create(1, "Cheeseburger", null, 5m);
        dish.AddRating("someone", RatingValue.Create(5), null);
        context.Dishes.Add(dish);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new SubmitRestaurantRatingCommandHandler(context);
        await handler.Handle(new SubmitRestaurantRatingCommand(restaurant.Id, "user1", 1, null), CancellationToken.None);

        var reloadedDish = await context.Dishes.FindAsync(dish.Id);
        Assert.Equal(5m, reloadedDish!.AverageRating);
    }

    [Fact]
    public async Task GetAverageRestaurantRating_ForRestaurantWithMultipleRatings_ReturnsCorrectAverage()
    {
        using var context = ApplicationDbContextFactory.Create();
        var restaurant = Restaurant.Create("Burger King", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        restaurant.AddRating("u1", RatingValue.Create(5), null);
        restaurant.AddRating("u2", RatingValue.Create(3), null);
        context.Restaurants.Add(restaurant);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetAverageRestaurantRatingQueryHandler(context);
        var average = await handler.Handle(new GetAverageRestaurantRatingQuery(restaurant.Id), CancellationToken.None);

        Assert.Equal(4m, average);
    }

    [Fact]
    public async Task GetRestaurantRatings_ReturnsAllRatingsForThatRestaurantOnly()
    {
        using var context = ApplicationDbContextFactory.Create();
        var restaurant1 = Restaurant.Create("R1", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        restaurant1.AddRating("u1", RatingValue.Create(5), null);
        restaurant1.AddRating("u2", RatingValue.Create(4), null);
        var restaurant2 = Restaurant.Create("R2", "Addr", PriceRange.Cheap, GeoLocation.Create(0, 0));
        restaurant2.AddRating("u3", RatingValue.Create(1), null);
        context.Restaurants.AddRange(restaurant1, restaurant2);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetRestaurantRatingsQueryHandler(context);
        var restaurant1Ratings = await handler.Handle(new GetRestaurantRatingsQuery(restaurant1.Id), CancellationToken.None);

        Assert.Equal(2, restaurant1Ratings.Count);
    }
}
