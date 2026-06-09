namespace LunchNear.Tests;

using LunchNear.Shared.Models;

public class DishTests
{
    [Fact]
    public void Dish_ShouldCreateWithValidData()
    {
        // Arrange
        var name = "Cheeseburger";
        var restaurantId = 1;
        var price = 8.99m;
        var rating = 4.2m;
        var description = "Tasty burger with cheese";

        // Act
        var dish = new Dish
        {
            Id = 1,
            Name = name,
            RestaurantId = restaurantId,
            Price = price,
            Rating = rating,
            Description = description
        };

        // Assert
        Assert.Equal(1, dish.Id);
        Assert.Equal(name, dish.Name);
        Assert.Equal(restaurantId, dish.RestaurantId);
        Assert.Equal(price, dish.Price);
        Assert.Equal(rating, dish.Rating);
        Assert.Equal(description, dish.Description);
    }

    [Fact]
    public void Dish_ShouldAllowNullDescription()
    {
        // Arrange & Act
        var dish = new Dish
        {
            Id = 2,
            Name = "Pizza",
            RestaurantId = 1,
            Price = 12.50m,
            Description = null
        };

        // Assert
        Assert.Null(dish.Description);
    }

    [Fact]
    public void Dish_RatingShouldStartAtZero()
    {
        // Arrange & Act
        var dish = new Dish
        {
            Id = 3,
            Name = "Salad",
            RestaurantId = 2,
            Price = 5.99m
        };

        // Assert
        Assert.Equal(0m, dish.Rating);
    }

    [Fact]
    public void Dish_ShouldHaveCorrectRestaurantRelationship()
    {
        // Arrange
        var restaurantId = 5;

        // Act
        var dish = new Dish
        {
            Id = 4,
            Name = "Pasta",
            RestaurantId = restaurantId,
            Price = 10.00m
        };

        // Assert
        Assert.Equal(restaurantId, dish.RestaurantId);
    }
}
