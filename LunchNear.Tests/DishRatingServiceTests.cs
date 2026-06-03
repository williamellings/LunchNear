namespace LunchNear.Tests;

using LunchNear.Models;
using LunchNear.Services;

public class DishRatingServiceTests
{
    private readonly IDishRatingService _service;
    private readonly IRestaurantService _restaurantService;

    public DishRatingServiceTests()
    {
        _restaurantService = new RestaurantService();
        _service = new DishRatingService(_restaurantService);
    }

    [Fact]
    public async Task SubmitDishRating_ShouldSaveRating()
    {
        // Arrange
        var rating = new DishRating
        {
            DishId = 1,
            UserId = "user1@example.com",
            Rating = 5,
            Review = "Amazing dish!"
        };

        // Act
        await _service.SubmitDishRating(rating);
        var savedRatings = await _service.GetDishRatings(1);

        // Assert
        Assert.Single(savedRatings);
        Assert.Equal("Amazing dish!", savedRatings.First().Review);
    }

    [Fact]
    public async Task SubmitDishRating_ShouldGenerateId()
    {
        // Arrange
        var rating1 = new DishRating { DishId = 1, UserId = "user1", Rating = 5 };
        var rating2 = new DishRating { DishId = 1, UserId = "user2", Rating = 4 };

        // Act
        await _service.SubmitDishRating(rating1);
        await _service.SubmitDishRating(rating2);

        // Assert
        var ratings = await _service.GetDishRatings(1);
        Assert.Equal(2, ratings.Count());
        Assert.NotEqual(rating1.Id, rating2.Id);
    }

    [Fact]
    public async Task SubmitDishRating_ShouldSetCreatedAt()
    {
        // Arrange
        var beforeSubmit = DateTime.Now;
        var rating = new DishRating { DishId = 1, UserId = "user", Rating = 5 };

        // Act
        await _service.SubmitDishRating(rating);
        var afterSubmit = DateTime.Now;

        // Assert
        Assert.True(rating.CreatedAt >= beforeSubmit && rating.CreatedAt <= afterSubmit);
    }

    [Fact]
    public async Task GetDishRatings_ShouldReturnAllRatingsForDish()
    {
        // Arrange
        await _service.SubmitDishRating(new DishRating { DishId = 1, UserId = "user1", Rating = 5 });
        await _service.SubmitDishRating(new DishRating { DishId = 1, UserId = "user2", Rating = 4 });
        await _service.SubmitDishRating(new DishRating { DishId = 2, UserId = "user3", Rating = 3 });

        // Act
        var dishOneRatings = await _service.GetDishRatings(1);
        var dishTwoRatings = await _service.GetDishRatings(2);

        // Assert
        Assert.Equal(2, dishOneRatings.Count());
        Assert.Single(dishTwoRatings);
    }

    [Fact]
    public async Task GetDishRatings_ShouldReturnEmptyForNonExistentDish()
    {
        // Act
        var ratings = await _service.GetDishRatings(999);

        // Assert
        Assert.Empty(ratings);
    }

    [Fact]
    public async Task CalculateAverageDishRating_ShouldReturnZeroForNoRatings()
    {
        // Act
        var average = await _service.CalculateAverageDishRating(999);

        // Assert
        Assert.Equal(0m, average);
    }

    [Fact]
    public async Task CalculateAverageDishRating_ShouldCalculateCorrectly()
    {
        // Arrange
        await _service.SubmitDishRating(new DishRating { DishId = 1, UserId = "user1", Rating = 5 });
        await _service.SubmitDishRating(new DishRating { DishId = 1, UserId = "user2", Rating = 4 });
        await _service.SubmitDishRating(new DishRating { DishId = 1, UserId = "user3", Rating = 3 });

        // Act
        var average = await _service.CalculateAverageDishRating(1);

        // Assert
        Assert.Equal(4m, average);
    }

    [Fact]
    public async Task CalculateAverageDishRating_ShouldHandleDecimalRatings()
    {
        // Arrange
        await _service.SubmitDishRating(new DishRating { DishId = 1, UserId = "user1", Rating = 5 });
        await _service.SubmitDishRating(new DishRating { DishId = 1, UserId = "user2", Rating = 4 });

        // Act
        var average = await _service.CalculateAverageDishRating(1);

        // Assert
        Assert.Equal(4.5m, average);
    }

    [Fact]
    public async Task CalculateAverageDishRating_ShouldOnlyCountSpecificDish()
    {
        // Arrange
        await _service.SubmitDishRating(new DishRating { DishId = 1, UserId = "user1", Rating = 5 });
        await _service.SubmitDishRating(new DishRating { DishId = 2, UserId = "user2", Rating = 1 });

        // Act
        var average = await _service.CalculateAverageDishRating(1);

        // Assert
        Assert.Equal(5m, average);
    }

    [Fact]
    public async Task GetTopRatedDishesByRestaurant_ShouldReturnEmptyForNonExistentRestaurant()
    {
        // Act
        var topDishes = await _service.GetTopRatedDishesByRestaurant(999);

        // Assert
        Assert.Empty(topDishes);
    }

    [Fact]
    public async Task GetTopRatedDishesByRestaurant_ShouldReturnDishesSortedByRating()
    {
        // Arrange - добавим оценки для разных блюд
        await _service.SubmitDishRating(new DishRating { DishId = 1, UserId = "user1", Rating = 3 });
        await _service.SubmitDishRating(new DishRating { DishId = 2, UserId = "user2", Rating = 5 });

        // Act
        var topDishes = await _service.GetTopRatedDishesByRestaurant(1);

        // Assert
        Assert.NotEmpty(topDishes);
        var dishesList = topDishes.ToList();
        if (dishesList.Count > 1)
        {
            Assert.True(dishesList[0].Rating >= dishesList[1].Rating);
        }
    }
}
