namespace LunchNear.Infrastructure.Persistence.Configurations;

using LunchNear.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("Restaurants");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Address).IsRequired().HasMaxLength(300);

        builder.Property(r => r.PriceRange)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.AverageRating).HasPrecision(3, 2);

        builder.OwnsOne(r => r.Location, location =>
        {
            location.Property(l => l.Latitude).HasColumnName("Latitude").IsRequired();
            location.Property(l => l.Longitude).HasColumnName("Longitude").IsRequired();
        });

        builder.Navigation(r => r.StudentDiscounts).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(r => r.LunchDeals).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(r => r.Ratings).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(r => r.StudentDiscounts)
            .WithOne(d => d.Restaurant)
            .HasForeignKey(d => d.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.LunchDeals)
            .WithOne(d => d.Restaurant)
            .HasForeignKey(d => d.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Ratings)
            .WithOne(rt => rt.Restaurant)
            .HasForeignKey(rt => rt.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
