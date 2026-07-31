namespace LunchNear.Infrastructure.Persistence.Configurations;

using LunchNear.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable("Dishes");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Description).HasMaxLength(1000);
        builder.Property(d => d.Price).HasPrecision(10, 2);
        builder.Property(d => d.AverageRating).HasPrecision(3, 2);

        builder.Navigation(d => d.Ratings).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(d => d.Restaurant)
            .WithMany()
            .HasForeignKey(d => d.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Ratings)
            .WithOne(r => r.Dish)
            .HasForeignKey(r => r.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.RestaurantId);
    }
}
