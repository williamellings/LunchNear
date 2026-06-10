namespace LunchNear.Tests;

using LunchNear.Shared.Models;
using LunchNear.Shared.Services;
using LunchNear.API.Services;
using LunchNear.Tests.Helpers;

public class RestaurantServiceTests
{
    private readonly IRestaurantService _service;

    public RestaurantServiceTests()
    {
        var dbContext = TestDbContextFactory.CreateTestDbContext();
        _service = new RestaurantService(dbContext);
    }

    [Fact]
    public async Task GetAllRestaurants_ShouldReturnAllRestaurants()
    {
        // Act
        var restaurants = await _service.GetAllRestaurants();

        // Assert
        Assert.NotEmpty(restaurants);
        Assert.Equal(8, restaurants.Count());
    }

    [Fact]
    public async Task GetAllRestaurants_ShouldReturnValidRestaurantData()
    {
        // Act
        var restaurants = await _service.GetAllRestaurants();
        var firstRestaurant = restaurants.First();

        // Assert
        Assert.NotNull(firstRestaurant.Name);
        Assert.NotNull(firstRestaurant.Address);
        Assert.NotNull(firstRestaurant.PriceRange);
        Assert.True(firstRestaurant.Id > 0);
    }

    [Fact]
    public async Task GetRestaurantById_ShouldReturnCorrectRestaurant()
    {
        // Act
        var restaurant = await _service.GetRestaurantById(1);

        // Assert
        Assert.NotNull(restaurant);
        Assert.Equal(1, restaurant.Id);
        Assert.Equal("Burger King", restaurant.Name);
        Assert.Equal("Avenyn, Gothenburg", restaurant.Address);
    }

    [Fact]
    public async Task GetRestaurantById_ShouldReturnNullForNonExistentId()
    {
        // Act
        var restaurant = await _service.GetRestaurantById(999);

        // Assert
        Assert.Null(restaurant);
    }

    [Fact]
    public async Task SearchRestaurants_ShouldFindByName()
    {
        // Act
        var results = await _service.SearchRestaurants("Burger");

        // Assert
        Assert.Single(results);
        Assert.Equal("Burger King", results.First().Name);
    }

    [Fact]
    public async Task SearchRestaurants_ShouldBeCaseInsensitive()
    {
        // Act
        var resultsLower = await _service.SearchRestaurants("pizzeria");
        var resultsUpper = await _service.SearchRestaurants("PIZZERIA");
        var resultsMixed = await _service.SearchRestaurants("PiZzErIa");

        // Assert
        Assert.Single(resultsLower);
        Assert.Single(resultsUpper);
        Assert.Single(resultsMixed);
        Assert.Equal(resultsLower.First().Id, resultsUpper.First().Id);
        Assert.Equal(resultsUpper.First().Id, resultsMixed.First().Id);
    }

    [Fact]
    public async Task SearchRestaurants_ShouldReturnMultipleMatches()
    {
        // Act
        var results = await _service.SearchRestaurants("a");

        // Assert
        Assert.NotEmpty(results);
        Assert.True(results.Count() > 1);
    }

    [Fact]
    public async Task SearchRestaurants_ShouldReturnEmptyForNoMatches()
    {
        // Act
        var results = await _service.SearchRestaurants("NonExistentRestaurant");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchRestaurants_ShouldReturnAllForEmptyQuery()
    {
        // Act
        var results = await _service.SearchRestaurants("");

        // Assert
        Assert.NotEmpty(results);
        Assert.Equal(8, results.Count());
    }

    [Fact]
    public async Task SearchRestaurants_ShouldTrimWhitespace()
    {
        // Act
        var results = await _service.SearchRestaurants("  Burger  ");

        // Assert
        Assert.Single(results);
        Assert.Equal("Burger King", results.First().Name);
    }

    [Fact]
    public async Task GetAllRestaurants_ShouldIncludeStudentDiscountInfo()
    {
        // Act
        var restaurants = await _service.GetAllRestaurants();
        var restaurantWithDiscount = restaurants.FirstOrDefault(r => r.HasStudentDiscount);

        // Assert
        Assert.NotNull(restaurantWithDiscount);
        Assert.True(restaurantWithDiscount.HasStudentDiscount);
    }

    [Fact]
    public async Task GetAllRestaurants_ShouldIncludeLunchBuffetInfo()
    {
        // Act
        var restaurants = await _service.GetAllRestaurants();
        var restaurantWithBuffet = restaurants.FirstOrDefault(r => r.HasLunchBuffet);

        // Assert
        Assert.NotNull(restaurantWithBuffet);
        Assert.True(restaurantWithBuffet.HasLunchBuffet);
    }

    [Fact]
    public async Task GetAllRestaurants_ShouldHaveValidRatings()
    {
        // Act
        var restaurants = await _service.GetAllRestaurants();

        // Assert
        foreach (var restaurant in restaurants)
        {
            Assert.True(restaurant.Rating >= 0);
            Assert.True(restaurant.Rating <= 5);
        }
    }
}
