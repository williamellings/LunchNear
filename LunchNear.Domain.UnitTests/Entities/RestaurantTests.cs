namespace LunchNear.Domain.UnitTests.Entities;

using LunchNear.Domain.Entities;
using LunchNear.Domain.Enums;
using LunchNear.Domain.Exceptions;
using LunchNear.Domain.ValueObjects;

public class RestaurantTests
{
    private static readonly GeoLocation ValidLocation = GeoLocation.Create(57.7089, 11.9746);

    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var restaurant = Restaurant.Create("Burger King", "Avenyn, Gothenburg", PriceRange.Cheap, ValidLocation);

        Assert.Equal("Burger King", restaurant.Name);
        Assert.Equal("Avenyn, Gothenburg", restaurant.Address);
        Assert.Equal(PriceRange.Cheap, restaurant.PriceRange);
        Assert.Equal(ValidLocation, restaurant.Location);
        Assert.Equal(0m, restaurant.AverageRating);
        Assert.Equal(0, restaurant.RatingsCount);
        Assert.False(restaurant.HasStudentDiscount);
        Assert.False(restaurant.HasLunchDeals);
    }

    [Theory]
    [InlineData("", "Some address")]
    [InlineData(" ", "Some address")]
    [InlineData(null, "Some address")]
    public void Create_WithoutName_ThrowsDomainException(string? name, string address)
    {
        Assert.Throws<DomainException>(() => Restaurant.Create(name!, address, PriceRange.Cheap, ValidLocation));
    }

    [Theory]
    [InlineData("Name", "")]
    [InlineData("Name", " ")]
    [InlineData("Name", null)]
    public void Create_WithoutAddress_ThrowsDomainException(string name, string? address)
    {
        Assert.Throws<DomainException>(() => Restaurant.Create(name, address!, PriceRange.Cheap, ValidLocation));
    }

    [Fact]
    public void AddRating_First_SetsAverageToThatRating()
    {
        var restaurant = Restaurant.Create("Test", "Addr", PriceRange.Medium, ValidLocation);

        restaurant.AddRating("user1", RatingValue.Create(4), "Good");

        Assert.Equal(4m, restaurant.AverageRating);
        Assert.Equal(1, restaurant.RatingsCount);
        Assert.Single(restaurant.Ratings);
    }

    [Fact]
    public void AddRating_Multiple_ComputesRunningAverage()
    {
        var restaurant = Restaurant.Create("Test", "Addr", PriceRange.Medium, ValidLocation);

        restaurant.AddRating("user1", RatingValue.Create(5), null);
        restaurant.AddRating("user2", RatingValue.Create(3), null);

        Assert.Equal(4m, restaurant.AverageRating);
        Assert.Equal(2, restaurant.RatingsCount);
    }

    [Fact]
    public void AddStudentDiscount_AddsToCollectionAndSetsFlag()
    {
        var restaurant = Restaurant.Create("Test", "Addr", PriceRange.Medium, ValidLocation);

        restaurant.AddStudentDiscount("15% off with student ID", 15);

        Assert.True(restaurant.HasStudentDiscount);
        Assert.Single(restaurant.StudentDiscounts);
        Assert.Equal(15, restaurant.StudentDiscounts.First().DiscountPercentage);
    }

    [Fact]
    public void AddLunchDeal_AddsToCollectionAndSetsFlag()
    {
        var restaurant = Restaurant.Create("Test", "Addr", PriceRange.Medium, ValidLocation);

        restaurant.AddLunchDeal(
            "Business Lunch",
            "Daily lunch offer",
            99m,
            new TimeOnly(12, 0),
            new TimeOnly(16, 0),
            "Mon-Fri");

        Assert.True(restaurant.HasLunchDeals);
        Assert.Single(restaurant.LunchDeals);
    }

    [Fact]
    public void AddLunchDeal_WithEndTimeBeforeStartTime_ThrowsDomainException()
    {
        var restaurant = Restaurant.Create("Test", "Addr", PriceRange.Medium, ValidLocation);

        Assert.Throws<DomainException>(() => restaurant.AddLunchDeal(
            "Bad Deal", null, 99m, new TimeOnly(16, 0), new TimeOnly(12, 0), "Mon-Fri"));
    }

    [Fact]
    public void AddStudentDiscount_WithInvalidPercentage_ThrowsDomainException()
    {
        var restaurant = Restaurant.Create("Test", "Addr", PriceRange.Medium, ValidLocation);

        Assert.Throws<DomainException>(() => restaurant.AddStudentDiscount("Bad", 0));
        Assert.Throws<DomainException>(() => restaurant.AddStudentDiscount("Bad", 101));
    }
}
