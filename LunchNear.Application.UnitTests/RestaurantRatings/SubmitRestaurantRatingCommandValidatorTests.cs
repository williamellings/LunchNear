namespace LunchNear.Application.UnitTests.RestaurantRatings;

using LunchNear.Application.RestaurantRatings.Commands;

public class SubmitRestaurantRatingCommandValidatorTests
{
    private readonly SubmitRestaurantRatingCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task Validate_WithRatingOutOfRange_HasError(int rating)
    {
        var result = await _validator.ValidateAsync(new SubmitRestaurantRatingCommand(1, "user1", rating, null));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithEmptyUserId_HasError()
    {
        var result = await _validator.ValidateAsync(new SubmitRestaurantRatingCommand(1, "", 5, null));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithValidCommand_IsValid()
    {
        var result = await _validator.ValidateAsync(new SubmitRestaurantRatingCommand(1, "user1", 5, "Great!"));

        Assert.True(result.IsValid);
    }
}
