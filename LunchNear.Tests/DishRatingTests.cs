namespace LunchNear.Tests;

using LunchNear.Shared.Models;

public class DishRatingTests
{
    [Fact]
    public void DishRating_ShouldCreateWithValidData()
    {
        // Arrange
        var dishId = 1;
        var userId = "user@example.com";
        var rating = 5;
        var review = "Amazing dish! Highly recommend.";
        var createdAt = DateTime.Now;

        // Act
        var dishRating = new DishRating
        {
            Id = 1,
            DishId = dishId,
            UserId = userId,
            Rating = rating,
            Review = review,
            CreatedAt = createdAt
        };

        // Assert
        Assert.Equal(1, dishRating.Id);
        Assert.Equal(dishId, dishRating.DishId);
        Assert.Equal(userId, dishRating.UserId);
        Assert.Equal(rating, dishRating.Rating);
        Assert.Equal(review, dishRating.Review);
        Assert.Equal(createdAt, dishRating.CreatedAt);
    }

    [Fact]
    public void DishRating_ShouldAllowNullReview()
    {
        // Arrange & Act
        var dishRating = new DishRating
        {
            Id = 2,
            DishId = 1,
            UserId = "user2@example.com",
            Rating = 4,
            Review = null
        };

        // Assert
        Assert.Null(dishRating.Review);
    }

    [Fact]
    public void DishRating_RatingShouldBeInValidRange()
    {
        // Arrange
        var validRatings = new[] { 1, 2, 3, 4, 5 };

        foreach (var rating in validRatings)
        {
            // Act
            var dishRating = new DishRating
            {
                Id = 3,
                DishId = 1,
                UserId = "user@example.com",
                Rating = rating
            };

            // Assert
            Assert.True(dishRating.Rating >= 1 && dishRating.Rating <= 5);
        }
    }

    [Fact]
    public void DishRating_ShouldTrackCreationTime()
    {
        // Arrange
        var beforeCreation = DateTime.Now.AddSeconds(-1);

        // Act
        var dishRating = new DishRating
        {
            Id = 4,
            DishId = 1,
            UserId = "user@example.com",
            Rating = 3,
            CreatedAt = DateTime.Now
        };

        var afterCreation = DateTime.Now.AddSeconds(1);

        // Assert
        Assert.True(dishRating.CreatedAt >= beforeCreation && dishRating.CreatedAt <= afterCreation);
    }
}
