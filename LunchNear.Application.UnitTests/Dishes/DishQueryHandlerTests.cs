namespace LunchNear.Application.UnitTests.Dishes;

using LunchNear.Application.Dishes.Queries;
using LunchNear.Application.UnitTests.Common;
using LunchNear.Domain.Entities;
using LunchNear.Domain.ValueObjects;

public class DishQueryHandlerTests
{
    [Fact]
    public async Task GetDishById_ExistingId_ReturnsDto()
    {
        using var context = ApplicationDbContextFactory.Create();
        var dish = Dish.Create(1, "Cheeseburger", "Tasty", 8.99m);
        context.Dishes.Add(dish);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetDishByIdQueryHandler(context);
        var result = await handler.Handle(new GetDishByIdQuery(dish.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Cheeseburger", result!.Name);
    }

    [Fact]
    public async Task GetDishById_NonExistentId_ReturnsNull()
    {
        using var context = ApplicationDbContextFactory.Create();

        var handler = new GetDishByIdQueryHandler(context);
        var result = await handler.Handle(new GetDishByIdQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetDishesByRestaurant_ReturnsOnlyDishesForThatRestaurant_OrderedByRatingDescending()
    {
        using var context = ApplicationDbContextFactory.Create();
        var dishHigh = Dish.Create(1, "Best Dish", null, 10m);
        dishHigh.AddRating("u1", RatingValue.Create(5), null);
        var dishLow = Dish.Create(1, "Ok Dish", null, 8m);
        dishLow.AddRating("u2", RatingValue.Create(2), null);
        var otherRestaurantDish = Dish.Create(2, "Other Restaurant Dish", null, 5m);

        context.Dishes.AddRange(dishHigh, dishLow, otherRestaurantDish);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetDishesByRestaurantQueryHandler(context);
        var result = await handler.Handle(new GetDishesByRestaurantQuery(1), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Best Dish", result[0].Name);
        Assert.Equal("Ok Dish", result[1].Name);
    }

    [Fact]
    public async Task GetTopRatedDishes_ExcludesUnratedDishes_AndRespectsTakeLimit()
    {
        using var context = ApplicationDbContextFactory.Create();
        var rated1 = Dish.Create(1, "Rated 1", null, 10m);
        rated1.AddRating("u1", RatingValue.Create(5), null);
        var rated2 = Dish.Create(1, "Rated 2", null, 10m);
        rated2.AddRating("u2", RatingValue.Create(4), null);
        var unrated = Dish.Create(1, "Unrated", null, 10m);

        context.Dishes.AddRange(rated1, rated2, unrated);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetTopRatedDishesQueryHandler(context);
        var result = await handler.Handle(new GetTopRatedDishesQuery(1, Take: 1), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Rated 1", result[0].Name);
    }

    [Fact]
    public async Task GetTopRatedDishes_ForRestaurantWithNoRatings_ReturnsEmpty()
    {
        using var context = ApplicationDbContextFactory.Create();
        context.Dishes.Add(Dish.Create(1, "Unrated", null, 10m));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetTopRatedDishesQueryHandler(context);
        var result = await handler.Handle(new GetTopRatedDishesQuery(999), CancellationToken.None);

        Assert.Empty(result);
    }
}
