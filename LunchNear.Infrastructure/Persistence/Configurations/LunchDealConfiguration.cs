namespace LunchNear.Infrastructure.Persistence.Configurations;

using LunchNear.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LunchDealConfiguration : IEntityTypeConfiguration<LunchDeal>
{
    public void Configure(EntityTypeBuilder<LunchDeal> builder)
    {
        builder.ToTable("LunchDeals");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Description).HasMaxLength(1000);
        builder.Property(d => d.Price).HasPrecision(10, 2);
        builder.Property(d => d.DaysOfWeek).IsRequired().HasMaxLength(100);

        builder.HasIndex(d => d.RestaurantId);
    }
}
