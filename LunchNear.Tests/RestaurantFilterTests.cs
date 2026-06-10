namespace LunchNear.Tests;

using LunchNear.Shared.Models;
using LunchNear.Shared.Services;
using LunchNear.API.Services;
using LunchNear.Tests.Helpers;

public class RestaurantFilterTests
{
    private readonly IRestaurantService _service;

    public RestaurantFilterTests()
    {
        var dbContext = TestDbContextFactory.CreateTestDbContext();
        _service = new RestaurantService(dbContext);
    }

    [Fact]
    public async Task FilterRestaurants_WithNullCriteria_ShouldReturnAllRestaurants()
    {
        // Arrange
        RestaurantFilterCriteria? criteria = null;

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.Equal(8, results.Count());
    }

    [Fact]
    public async Task FilterRestaurants_WithEmptyCriteria_ShouldReturnAllRestaurants()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria();

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.Equal(8, results.Count());
    }

    [Fact]
    public async Task FilterRestaurants_ByPriceOnly_ShouldReturnOnlyThatPrice()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria { PriceRange = "Cheap" };

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Equal("Cheap", r.PriceRange));
    }

    [Fact]
    public async Task FilterRestaurants_ByStudentDiscount_ShouldReturnOnlyWithDiscount()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria { HasStudentDiscount = true };

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.True(r.HasStudentDiscount));
    }

    [Fact]
    public async Task FilterRestaurants_ByLunchBuffet_ShouldReturnOnlyWithBuffet()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria { HasLunchBuffet = true };

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.True(r.HasLunchBuffet));
    }

    [Fact]
    public async Task FilterRestaurants_ByPriceAndDiscount_ShouldReturnMatching()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria
        {
            PriceRange = "Cheap",
            HasStudentDiscount = true
        };

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.NotEmpty(results);
        Assert.All(results, r =>
        {
            Assert.Equal("Cheap", r.PriceRange);
            Assert.True(r.HasStudentDiscount);
        });
    }

    [Fact]
    public async Task FilterRestaurants_ByPriceAndBuffet_ShouldReturnMatching()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria
        {
            PriceRange = "Medium",
            HasLunchBuffet = true
        };

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.NotEmpty(results);
        Assert.All(results, r =>
        {
            Assert.Equal("Medium", r.PriceRange);
            Assert.True(r.HasLunchBuffet);
        });
    }

    [Fact]
    public async Task FilterRestaurants_ByAllThree_ShouldReturnMatching()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria
        {
            PriceRange = "Medium",
            HasStudentDiscount = true,
            HasLunchBuffet = true
        };

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.All(results, r =>
        {
            Assert.Equal("Medium", r.PriceRange);
            Assert.True(r.HasStudentDiscount);
            Assert.True(r.HasLunchBuffet);
        });
    }

    [Fact]
    public async Task FilterRestaurants_ByInvalidPrice_ShouldReturnEmpty()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria { PriceRange = "VeryExpensive" };

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task FilterRestaurants_ByDiscount_False_ShouldReturnWithoutDiscount()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria { HasStudentDiscount = false };

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.False(r.HasStudentDiscount));
    }

    [Fact]
    public async Task FilterRestaurants_ByBuffet_False_ShouldReturnWithoutBuffet()
    {
        // Arrange
        var criteria = new RestaurantFilterCriteria { HasLunchBuffet = false };

        // Act
        var results = await _service.FilterRestaurants(criteria);

        // Assert
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.False(r.HasLunchBuffet));
    }
}
