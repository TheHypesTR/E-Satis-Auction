using e_Sat_Auction.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_Sat_Auction.Data.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.Property(category => category.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(category => category.NormalizedName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(category => category.Description)
            .HasMaxLength(500);

        builder.HasIndex(category => category.Name);
        builder.HasIndex(category => category.NormalizedName)
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique();
        builder.HasIndex(category => category.IsActive);

        builder.HasMany(category => category.Attributes)
            .WithOne(attribute => attribute.Category)
            .HasForeignKey(attribute => attribute.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(category => category.Attributes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}