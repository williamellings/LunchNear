namespace LunchNear.Application.UnitTests.DishRatings;

using LunchNear.Application.DishRatings.Commands;

public class SubmitDishRatingCommandValidatorTests
{
    private readonly SubmitDishRatingCommandValidator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public async Task Validate_WithRatingInRange_HasNoErrorsForRating(int rating)
    {
        var result = await _validator.ValidateAsync(new SubmitDishRatingCommand(1, "user1", rating, null));

        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(SubmitDishRatingCommand.Rating));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public async Task Validate_WithRatingOutOfRange_HasError(int rating)
    {
        var result = await _validator.ValidateAsync(new SubmitDishRatingCommand(1, "user1", rating, null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(SubmitDishRatingCommand.Rating));
    }

    [Fact]
    public async Task Validate_WithEmptyUserId_HasError()
    {
        var result = await _validator.ValidateAsync(new SubmitDishRatingCommand(1, "", 5, null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(SubmitDishRatingCommand.UserId));
    }

    [Fact]
    public async Task Validate_WithZeroOrNegativeDishId_HasError()
    {
        var result = await _validator.ValidateAsync(new SubmitDishRatingCommand(0, "user1", 5, null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(SubmitDishRatingCommand.DishId));
    }

    [Fact]
    public async Task Validate_WithReviewOverMaxLength_HasError()
    {
        var longReview = new string('a', 1001);

        var result = await _validator.ValidateAsync(new SubmitDishRatingCommand(1, "user1", 5, longReview));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(SubmitDishRatingCommand.Review));
    }

    [Fact]
    public async Task Validate_WithValidCommand_IsValid()
    {
        var result = await _validator.ValidateAsync(new SubmitDishRatingCommand(1, "user1", 5, "Great!"));

        Assert.True(result.IsValid);
    }
}
