using E_Satis_Auction.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class CategoryAttributeOptionConfiguration : IEntityTypeConfiguration<CategoryAttributeOption>
{
    public void Configure(EntityTypeBuilder<CategoryAttributeOption> builder)
    {
        builder.ToTable("CategoryAttributeOptions");

        builder.Property(option => option.Value)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(option => option.CategoryAttributeId);
        builder.HasIndex(option => new { option.CategoryAttributeId, option.Value })
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique();
    }
}