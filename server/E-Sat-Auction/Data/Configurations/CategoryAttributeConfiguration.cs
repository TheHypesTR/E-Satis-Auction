using e_Sat_Auction.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_Sat_Auction.Data.Configurations;

public sealed class CategoryAttributeConfiguration : IEntityTypeConfiguration<CategoryAttribute>
{
    public void Configure(EntityTypeBuilder<CategoryAttribute> builder)
    {
        builder.ToTable("CategoryAttributes");

        builder.Property(attribute => attribute.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(attribute => attribute.Code)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(attribute => attribute.DataType)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(attribute => attribute.Target)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(attribute => attribute.CategoryId);
        builder.HasIndex(attribute => new { attribute.CategoryId, attribute.Code })
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique();

        builder.HasMany(attribute => attribute.Options)
            .WithOne(option => option.CategoryAttribute)
            .HasForeignKey(option => option.CategoryAttributeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(attribute => attribute.Options)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}