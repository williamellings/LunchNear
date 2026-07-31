namespace LunchNear.Domain.UnitTests.Entities;

using LunchNear.Domain.Entities;
using LunchNear.Domain.Exceptions;
using LunchNear.Domain.ValueObjects;

public class DishTests
{
    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var dish = Dish.Create(1, "Cheeseburger", "Tasty burger with cheese", 8.99m);

        Assert.Equal(1, dish.RestaurantId);
        Assert.Equal("Cheeseburger", dish.Name);
        Assert.Equal("Tasty burger with cheese", dish.Description);
        Assert.Equal(8.99m, dish.Price);
        Assert.Equal(0m, dish.AverageRating);
        Assert.Equal(0, dish.RatingsCount);
    }

    [Fact]
    public void Create_AllowsNullDescription()
    {
        var dish = Dish.Create(1, "Pizza", null, 12.50m);

        Assert.Null(dish.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidRestaurantId_ThrowsDomainException(int restaurantId)
    {
        Assert.Throws<DomainException>(() => Dish.Create(restaurantId, "Pasta", null, 10m));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithoutName_ThrowsDomainException(string? name)
    {
        Assert.Throws<DomainException>(() => Dish.Create(1, name!, null, 10m));
    }

    [Fact]
    public void Create_WithNegativePrice_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => Dish.Create(1, "Pasta", null, -0.01m));
    }

    [Fact]
    public void AddRating_First_SetsAverageToThatRating()
    {
        var dish = Dish.Create(1, "Salad", null, 5.99m);

        dish.AddRating("user1", RatingValue.Create(5), "Great");

        Assert.Equal(5m, dish.AverageRating);
        Assert.Equal(1, dish.RatingsCount);
    }

    [Fact]
    public void AddRating_Multiple_ComputesRunningAverageIndependentlyOfRestaurant()
    {
        var dish = Dish.Create(1, "Salad", null, 5.99m);

        dish.AddRating("user1", RatingValue.Create(5), null);
        dish.AddRating("user2", RatingValue.Create(4), null);
        dish.AddRating("user3", RatingValue.Create(3), null);

        Assert.Equal(4m, dish.AverageRating);
        Assert.Equal(3, dish.RatingsCount);
    }

    [Fact]
    public void AddRating_RoundsToTwoDecimalPlaces()
    {
        var dish = Dish.Create(1, "Soup", null, 4.5m);

        dish.AddRating("user1", RatingValue.Create(5), null);
        dish.AddRating("user2", RatingValue.Create(4), null);

        Assert.Equal(4.5m, dish.AverageRating);
    }
}
