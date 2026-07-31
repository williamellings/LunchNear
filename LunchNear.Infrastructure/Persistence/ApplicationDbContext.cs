namespace LunchNear.Infrastructure.Persistence;

using LunchNear.Application.Common.Interfaces;
using LunchNear.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<DishRating> DishRatings => Set<DishRating>();
    public DbSet<RestaurantRating> RestaurantRatings => Set<RestaurantRating>();
    public DbSet<StudentDiscount> StudentDiscounts => Set<StudentDiscount>();
    public DbSet<LunchDeal> LunchDeals => Set<LunchDeal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
