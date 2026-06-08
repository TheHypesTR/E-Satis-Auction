using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Facilities;
using E_Satis_Auction.Models.Items;
using E_Satis_Auction.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class PartSaleOperationConfiguration : IEntityTypeConfiguration<PartSaleOperation>
{
    public void Configure(EntityTypeBuilder<PartSaleOperation> builder)
    {
        builder.ToTable("PartSaleOperations");

        builder.Property(operation => operation.UnitOfMeasure)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(operation => operation.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(operation => operation.Notes)
            .HasMaxLength(1024);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(operation => operation.SourceItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(operation => operation.CreatedPartItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(operation => operation.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Facility>()
            .WithMany()
            .HasForeignKey(operation => operation.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(operation => operation.SourceItemId);
        builder.HasIndex(operation => operation.CreatedPartItemId);
        builder.HasIndex(operation => operation.CreatedAt);
    }
}
