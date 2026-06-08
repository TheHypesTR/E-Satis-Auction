using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Facilities;
using E_Satis_Auction.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class ProductListingConfiguration : IEntityTypeConfiguration<ProductListing>
{
    public void Configure(EntityTypeBuilder<ProductListing> builder)
    {
        builder.ToTable("ProductListings");

        builder.Property(listing => listing.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(listing => listing.SalePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(listing => listing.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(listing => listing.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(listing => listing.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Facility>()
            .WithMany()
            .HasForeignKey(listing => listing.SourceFacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(listing => listing.ProductId);
        builder.HasIndex(listing => listing.SourceFacilityId);
        builder.HasIndex(listing => listing.Status);
        builder.HasIndex(listing => new { listing.ProductId, listing.SourceFacilityId, listing.Status });
    }
}
