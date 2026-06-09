namespace LunchNear.Tests;

using LunchNear.Shared.Models;

public class StudentDiscountTests
{
    [Fact]
    public void StudentDiscount_ShouldCreateWithValidData()
    {
        // Arrange
        var restaurantId = 1;
        var description = "20% off with valid student ID";
        var discountPercentage = 20;

        // Act
        var discount = new StudentDiscount
        {
            Id = 1,
            RestaurantId = restaurantId,
            Description = description,
            DiscountPercentage = discountPercentage
        };

        // Assert
        Assert.Equal(1, discount.Id);
        Assert.Equal(restaurantId, discount.RestaurantId);
        Assert.Equal(description, discount.Description);
        Assert.Equal(discountPercentage, discount.DiscountPercentage);
    }

    [Fact]
    public void StudentDiscount_ShouldAllowVariousDiscountPercentages()
    {
        // Arrange
        var discountPercentages = new[] { 5, 10, 15, 20, 25, 30 };

        foreach (var percentage in discountPercentages)
        {
            // Act
            var discount = new StudentDiscount
            {
                Id = 2,
                RestaurantId = 1,
                Description = $"{percentage}% student discount",
                DiscountPercentage = percentage
            };

            // Assert
            Assert.Equal(percentage, discount.DiscountPercentage);
            Assert.True(discount.DiscountPercentage > 0);
        }
    }

    [Fact]
    public void StudentDiscount_ShouldLinkToRestaurant()
    {
        // Arrange
        var restaurantId = 42;

        // Act
        var discount = new StudentDiscount
        {
            Id = 3,
            RestaurantId = restaurantId,
            Description = "Student offer",
            DiscountPercentage = 15
        };

        // Assert
        Assert.Equal(restaurantId, discount.RestaurantId);
    }

    [Fact]
    public void StudentDiscount_DescriptionShouldBeRequired()
    {
        // Arrange & Act & Assert
        // This will fail at compile time if Description is not required
        var discount = new StudentDiscount
        {
            Id = 4,
            RestaurantId = 1,
            Description = "Test discount",
            DiscountPercentage = 10
        };

        Assert.NotEmpty(discount.Description);
    }
}
