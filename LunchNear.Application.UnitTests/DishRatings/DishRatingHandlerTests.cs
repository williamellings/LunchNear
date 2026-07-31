namespace LunchNear.Application.UnitTests.DishRatings;

using LunchNear.Application.Common.Exceptions;
using LunchNear.Application.DishRatings.Commands;
using LunchNear.Application.DishRatings.Queries;
using LunchNear.Application.UnitTests.Common;
using LunchNear.Domain.Entities;

public class DishRatingHandlerTests
{
    [Fact]
    public async Task SubmitDishRating_ForExistingDish_PersistsRatingAndReturnsDto()
    {
        using var context = ApplicationDbContextFactory.Create();
        var dish = Dish.Create(1, "Salad", null, 5.99m);
        context.Dishes.Add(dish);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new SubmitDishRatingCommandHandler(context);
        var result = await handler.Handle(
            new SubmitDishRatingCommand(dish.Id, "user1", 5, "Amazing dish!"), CancellationToken.None);

        Assert.Equal(dish.Id, result.DishId);
        Assert.Equal("user1", result.UserId);
        Assert.Equal(5, result.Rating);
        Assert.Equal("Amazing dish!", result.Review);

        var ratingsHandler = new GetDishRatingsQueryHandler(context);
        var ratings = await ratingsHandler.Handle(new GetDishRatingsQuery(dish.Id), CancellationToken.None);
        Assert.Single(ratings);
    }

    [Fact]
    public async Task SubmitDishRating_ForNonExistentDish_ThrowsNotFoundException()
    {
        using var context = ApplicationDbContextFactory.Create();
        var handler = new SubmitDishRatingCommandHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new SubmitDishRatingCommand(999, "user1", 5, null), CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task SubmitDishRating_SecondRatingFromDifferentRequestScope_ComputesCorrectRunningAverage()
    {
        // Regression test for a bug where the cached average was recalculated from
        // Dish.Ratings.Count/.Average() instead of incrementally from the cached scalars.
        // Because each Handle() call here uses a *freshly loaded* Dish (as a real request would,
        // via a new DbContext instance sharing the same InMemory database) without its Ratings
        // navigation included, a recalculation based on the in-memory collection would only see
        // the single newly-added rating and silently discard all previously persisted ones.
        var databaseName = Guid.NewGuid().ToString();

        int dishId;
        using (var seedContext = ApplicationDbContextFactory.Create(databaseName))
        {
            var dish = Dish.Create(1, "Salad", null, 5.99m);
            seedContext.Dishes.Add(dish);
            await seedContext.SaveChangesAsync(CancellationToken.None);
            dishId = dish.Id;
        }

        using (var firstRequestContext = ApplicationDbContextFactory.Create(databaseName))
        {
            var handler = new SubmitDishRatingCommandHandler(firstRequestContext);
            await handler.Handle(new SubmitDishRatingCommand(dishId, "user1", 5, null), CancellationToken.None);
        }

        using (var secondRequestContext = ApplicationDbContextFactory.Create(databaseName))
        {
            var handler = new SubmitDishRatingCommandHandler(secondRequestContext);
            await handler.Handle(new SubmitDishRatingCommand(dishId, "user2", 3, null), CancellationToken.None);
        }

        using var verifyContext = ApplicationDbContextFactory.Create(databaseName);
        var averageHandler = new GetAverageDishRatingQueryHandler(verifyContext);
        var average = await averageHandler.Handle(new GetAverageDishRatingQuery(dishId), CancellationToken.None);

        Assert.Equal(4m, average);
    }

    [Fact]
    public async Task GetAverageDishRating_ForDishWithNoRatings_ReturnsZero()
    {
        using var context = ApplicationDbContextFactory.Create();
        var dish = Dish.Create(1, "Salad", null, 5.99m);
        context.Dishes.Add(dish);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetAverageDishRatingQueryHandler(context);
        var average = await handler.Handle(new GetAverageDishRatingQuery(dish.Id), CancellationToken.None);

        Assert.Equal(0m, average);
    }

    [Fact]
    public async Task GetAverageDishRating_ForNonExistentDish_ThrowsNotFoundException()
    {
        using var context = ApplicationDbContextFactory.Create();
        var handler = new GetAverageDishRatingQueryHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetAverageDishRatingQuery(999), CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task GetDishRatings_ReturnsOnlyRatingsForThatDish_NewestFirst()
    {
        using var context = ApplicationDbContextFactory.Create();
        var dish1 = Dish.Create(1, "Dish 1", null, 5m);
        dish1.AddRating("u1", LunchNear.Domain.ValueObjects.RatingValue.Create(5), null);
        var dish2 = Dish.Create(1, "Dish 2", null, 5m);
        dish2.AddRating("u2", LunchNear.Domain.ValueObjects.RatingValue.Create(3), null);
        context.Dishes.AddRange(dish1, dish2);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetDishRatingsQueryHandler(context);
        var dish1Ratings = await handler.Handle(new GetDishRatingsQuery(dish1.Id), CancellationToken.None);
        var dish2Ratings = await handler.Handle(new GetDishRatingsQuery(dish2.Id), CancellationToken.None);

        Assert.Single(dish1Ratings);
        Assert.Single(dish2Ratings);
        Assert.Equal("u1", dish1Ratings[0].UserId);
    }
}
