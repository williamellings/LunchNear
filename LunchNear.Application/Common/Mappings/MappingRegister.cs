namespace LunchNear.Application.Common.Mappings;

using LunchNear.Contracts.Dishes;
using LunchNear.Contracts.LunchDeals;
using LunchNear.Contracts.Ratings;
using LunchNear.Contracts.Restaurants;
using LunchNear.Contracts.StudentDiscounts;
using LunchNear.Domain.Entities;
using Mapster;

/// <summary>
/// Explicit Domain -> Contracts mapping rules. Kept in one place so it is obvious, at a glance,
/// that clients only ever receive flat DTOs and never a raw EF Core entity graph.
/// </summary>
public sealed class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Restaurant, RestaurantDto>()
            .Map(dest => dest.PriceRange, src => src.PriceRange.ToString())
            .Map(dest => dest.Latitude, src => src.Location.Latitude)
            .Map(dest => dest.Longitude, src => src.Location.Longitude)
            .Map(
                dest => dest.StudentDiscountPercentage,
                src => src.StudentDiscounts.Count > 0
                    ? src.StudentDiscounts.Max(d => d.DiscountPercentage)
                    : (int?)null);

        config.NewConfig<Dish, DishDto>();

        config.NewConfig<RestaurantRating, RestaurantRatingDto>()
            .Map(dest => dest.Rating, src => src.Rating.Value);

        config.NewConfig<DishRating, DishRatingDto>()
            .Map(dest => dest.Rating, src => src.Rating.Value);

        config.NewConfig<StudentDiscount, StudentDiscountDto>();

        config.NewConfig<LunchDeal, LunchDealDto>();
    }
}
