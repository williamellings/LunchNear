namespace LunchNear.Tests;

using LunchNear.Models;
using LunchNear.Services;

public class StudentDiscountServiceTests
{
    private readonly IStudentDiscountService _service = new StudentDiscountService();

    [Fact]
    public async Task GetStudentDiscounts_ShouldReturnDiscountsForRestaurant()
    {
        // Act
        var discounts = await _service.GetStudentDiscounts(1);

        // Assert
        Assert.NotEmpty(discounts);
        Assert.All(discounts, d => Assert.Equal(1, d.RestaurantId));
    }

    [Fact]
    public async Task GetStudentDiscounts_ShouldReturnEmptyForRestaurantWithoutDiscount()
    {
        // Act
        var discounts = await _service.GetStudentDiscounts(3);

        // Assert
        Assert.Empty(discounts);
    }

    [Fact]
    public async Task GetStudentDiscounts_ShouldReturnCorrectDiscountPercentage()
    {
        // Act
        var discounts = await _service.GetStudentDiscounts(1);
        var firstDiscount = discounts.First();

        // Assert
        Assert.Equal(20, firstDiscount.DiscountPercentage);
    }

    [Fact]
    public async Task HasStudentDiscount_ShouldReturnTrueForRestaurantWithDiscount()
    {
        // Act
        var hasDiscount = await _service.HasStudentDiscount(1);

        // Assert
        Assert.True(hasDiscount);
    }

    [Fact]
    public async Task HasStudentDiscount_ShouldReturnFalseForRestaurantWithoutDiscount()
    {
        // Act
        var hasDiscount = await _service.HasStudentDiscount(3);

        // Assert
        Assert.False(hasDiscount);
    }

    [Fact]
    public async Task HasStudentDiscount_ShouldReturnFalseForNonExistentRestaurant()
    {
        // Act
        var hasDiscount = await _service.HasStudentDiscount(999);

        // Assert
        Assert.False(hasDiscount);
    }

    [Fact]
    public async Task GetRestaurantsWithStudentDiscounts_ShouldReturnOnlyWithDiscounts()
    {
        // Arrange
        var restaurants = new List<Restaurant>
        {
            new() { Id = 1, Name = "Restaurant 1", Address = "Address 1", PriceRange = "Cheap" },
            new() { Id = 2, Name = "Restaurant 2", Address = "Address 2", PriceRange = "Medium" },
            new() { Id = 3, Name = "Restaurant 3", Address = "Address 3", PriceRange = "Expensive" },
            new() { Id = 6, Name = "Restaurant 6", Address = "Address 6", PriceRange = "Medium" }
        };

        // Act
        var filtered = await _service.GetRestaurantsWithStudentDiscounts(restaurants);

        // Assert
        Assert.NotEmpty(filtered);
        foreach (var restaurant in filtered)
        {
            var hasDiscount = await _service.HasStudentDiscount(restaurant.Id);
            Assert.True(hasDiscount);
        }
    }

    [Fact]
    public async Task GetRestaurantsWithStudentDiscounts_ShouldReturnEmptyForNoMatches()
    {
        // Arrange
        var restaurants = new List<Restaurant>
        {
            new() { Id = 999, Name = "NonExistent", Address = "Address", PriceRange = "Cheap" }
        };

        // Act
        var filtered = await _service.GetRestaurantsWithStudentDiscounts(restaurants);

        // Assert
        Assert.Empty(filtered);
    }

    [Fact]
    public async Task GetRestaurantsWithStudentDiscounts_ShouldPreserveRestaurantData()
    {
        // Arrange
        var restaurants = new List<Restaurant>
        {
            new()
            {
                Id = 1,
                Name = "Burger King",
                Address = "Main Street",
                PriceRange = "Cheap",
                Rating = 4.5m
            }
        };

        // Act
        var filtered = await _service.GetRestaurantsWithStudentDiscounts(restaurants);

        // Assert
        var result = filtered.First();
        Assert.Equal("Burger King", result.Name);
        Assert.Equal("Main Street", result.Address);
        Assert.Equal(4.5m, result.Rating);
    }

    [Fact]
    public async Task StudentDiscounts_ShouldHaveVariousPercentages()
    {
        // Act
        var allDiscounts = new List<StudentDiscount>();
        for (int i = 1; i <= 8; i++)
        {
            var discounts = await _service.GetStudentDiscounts(i);
            allDiscounts.AddRange(discounts);
        }

        // Assert
        Assert.NotEmpty(allDiscounts);
        var percentages = allDiscounts.Select(d => d.DiscountPercentage).Distinct().ToList();
        Assert.True(percentages.Count > 1, "Should have multiple discount percentages");
    }

    [Fact]
    public async Task GetStudentDiscounts_ShouldIncludeDescription()
    {
        // Act
        var discounts = await _service.GetStudentDiscounts(1);
        var firstDiscount = discounts.First();

        // Assert
        Assert.NotEmpty(firstDiscount.Description);
        Assert.Contains("student", firstDiscount.Description.ToLower());
    }
}
