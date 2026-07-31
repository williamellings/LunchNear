namespace LunchNear.Application.Common.Interfaces;

using LunchNear.Domain.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Persistence boundary seen by the Application layer. Handlers query through this interface
/// with plain LINQ instead of a generic Repository&lt;T&gt; - EF Core's DbSet/change tracker
/// already are the repository/unit-of-work, wrapping them again would only add ceremony.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Restaurant> Restaurants { get; }
    DbSet<Dish> Dishes { get; }
    DbSet<DishRating> DishRatings { get; }
    DbSet<RestaurantRating> RestaurantRatings { get; }
    DbSet<StudentDiscount> StudentDiscounts { get; }
    DbSet<LunchDeal> LunchDeals { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
