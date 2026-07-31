namespace LunchNear.Infrastructure.Persistence.Configurations;

using LunchNear.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StudentDiscountConfiguration : IEntityTypeConfiguration<StudentDiscount>
{
    public void Configure(EntityTypeBuilder<StudentDiscount> builder)
    {
        builder.ToTable("StudentDiscounts");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Description).IsRequired().HasMaxLength(300);
        builder.Property(d => d.DiscountPercentage).IsRequired();

        builder.HasIndex(d => d.RestaurantId);
    }
}
