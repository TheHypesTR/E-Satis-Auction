using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.ToTable("PurchaseOrderLines");

        builder.Property(line => line.ProductNameSnapshot)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(line => line.SkuSnapshot)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(line => line.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(line => line.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(line => line.DiscountedUnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(line => line.FinalUnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(line => line.SubtotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(line => line.CouponDiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(line => line.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(line => line.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProductListing>()
            .WithMany()
            .HasForeignKey(line => line.ProductListingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Campaign>()
            .WithMany()
            .HasForeignKey(line => line.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(line => line.Allocations)
            .WithOne()
            .HasForeignKey(allocation => allocation.PurchaseOrderLineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(line => line.Allocations)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(line => line.PurchaseOrderId);
        builder.HasIndex(line => line.ProductId);
        builder.HasIndex(line => line.ProductListingId);
        builder.HasIndex(line => line.CampaignId);
    }
}
