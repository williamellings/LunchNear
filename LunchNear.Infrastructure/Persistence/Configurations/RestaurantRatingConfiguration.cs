namespace LunchNear.Infrastructure.Persistence.Configurations;

using LunchNear.Domain.Entities;
using LunchNear.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RestaurantRatingConfiguration : IEntityTypeConfiguration<RestaurantRating>
{
    public void Configure(EntityTypeBuilder<RestaurantRating> builder)
    {
        builder.ToTable("RestaurantRatings");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Review).HasMaxLength(1000);
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.Property(r => r.Rating)
            .HasConversion(rating => rating.Value, value => RatingValue.Create(value))
            .HasColumnName("Rating")
            .IsRequired();

        builder.HasIndex(r => r.RestaurantId);
    }
}
