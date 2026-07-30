namespace LunchNear.API.Data;

using Microsoft.EntityFrameworkCore;
using LunchNear.Shared.Models;

/// <summary>
/// Entity Framework Core DbContext for LunchNear application.
/// Manages database communication and entity mapping.
/// </summary>
public class LunchNearDbContext : DbContext
{
    public LunchNearDbContext(DbContextOptions<LunchNearDbContext> options) : base(options) { }

    public required DbSet<Restaurant> Restaurants { get; set; } 
    public required  DbSet<Dish> Dishes { get; set; }
    public required DbSet<DishRating> DishRatings { get; set; }
    public required DbSet<StudentDiscount> StudentDiscounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Restaurant>()
        .HasMany(r => r.Dishes)
        .WithOne(d => d.Restaurant)
        .HasForeignKey(d => d.RestaurantId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Restaurant>()
            .HasMany(r => r.StudentDiscounts)
            .WithOne(sd => sd.Restaurant)
            .HasForeignKey(sd => sd.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Dish>()
            .HasMany(d => d.Ratings)
            .WithOne(r => r.Dish)
            .HasForeignKey(r => r.DishId)
            .OnDelete(DeleteBehavior.Cascade);




        // Seed initial data
        SeedRestaurants(modelBuilder);
        SeedStudentDiscounts(modelBuilder);
        SeedDishes(modelBuilder);


    }

    private void SeedRestaurants(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>().HasData(
            new Restaurant { Id = 1, Name = "Burger King", Address = "Avenyn, Gothenburg", Rating = 3.8m, PriceRange = "Cheap", HasLunchBuffet = false, HasStudentDiscount = true },
            new Restaurant { Id = 2, Name = "Pizzeria Marco", Address = "Magasinsgatan, Gothenburg", Rating = 4.2m, PriceRange = "Medium", HasLunchBuffet = true, HasStudentDiscount = true },
            new Restaurant { Id = 3, Name = "Sushi Paradise", Address = "Kungsgatan, Gothenburg", Rating = 4.5m, PriceRange = "Expensive", HasLunchBuffet = true, HasStudentDiscount = false },
            new Restaurant { Id = 4, Name = "Tacos El Amigo", Address = "Nils Ericson Plats, Gothenburg", Rating = 4.1m, PriceRange = "Cheap", HasLunchBuffet = false, HasStudentDiscount = true },
            new Restaurant { Id = 5, Name = "Thai Kitchen", Address = "Jarntorget, Gothenburg", Rating = 4.3m, PriceRange = "Medium", HasLunchBuffet = true, HasStudentDiscount = true },
            new Restaurant { Id = 6, Name = "Steakhouse Premium", Address = "Storgatan, Gothenburg", Rating = 4.7m, PriceRange = "Expensive", HasLunchBuffet = false, HasStudentDiscount = false },
            new Restaurant { Id = 7, Name = "Kebab House", Address = "Heden, Gothenburg", Rating = 3.9m, PriceRange = "Cheap", HasLunchBuffet = false, HasStudentDiscount = true },
            new Restaurant { Id = 8, Name = "Pasta Perfetto", Address = "Vasaplatsen, Gothenburg", Rating = 4.4m, PriceRange = "Medium", HasLunchBuffet = true, HasStudentDiscount = false }
        );
    }

    private void SeedDishes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dish>().HasData(
            new Dish { Id = 1, Name = "Whopper", RestaurantId = 1, Price = 8.99m, Rating = 0m, Description = "Classic flame-grilled burger" },
            new Dish { Id = 2, Name = "Chicken Sandwich", RestaurantId = 1, Price = 7.99m, Rating = 0m, Description = "Crispy chicken sandwich" },
            new Dish { Id = 3, Name = "Margherita Pizza", RestaurantId = 2, Price = 12.99m, Rating = 0m, Description = "Classic Italian pizza" },
            new Dish { Id = 4, Name = "Quattro Formaggi", RestaurantId = 2, Price = 14.99m, Rating = 0m, Description = "Four cheese pizza" },
            new Dish { Id = 5, Name = "Salmon Roll", RestaurantId = 3, Price = 15.99m, Rating = 0m, Description = "Fresh salmon sushi roll" },
            new Dish { Id = 6, Name = "Dragon Roll", RestaurantId = 3, Price = 16.99m, Rating = 0m, Description = "Tempura shrimp and avocado" }
        );
    }

    private void SeedStudentDiscounts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentDiscount>().HasData(
            new StudentDiscount { Id = 1, RestaurantId = 1, Description = "20% off with valid student ID", DiscountPercentage = 20 },
            new StudentDiscount { Id = 2, RestaurantId = 2, Description = "Buy one pizza get second 50% off", DiscountPercentage = 50 },
            new StudentDiscount { Id = 3, RestaurantId = 4, Description = "15% student discount with ID", DiscountPercentage = 15 },
            new StudentDiscount { Id = 4, RestaurantId = 5, Description = "10% off for students every day", DiscountPercentage = 10 },
            new StudentDiscount { Id = 5, RestaurantId = 7, Description = "Free drink with student discount", DiscountPercentage = 5 }
        );
    }
}