using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("ShoppingCarts");

        builder.Property(cart => cart.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(cart => cart.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(cart => cart.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(cart => cart.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(cart => cart.PreviewSubtotalAmount).HasPrecision(18, 2);
        builder.Property(cart => cart.PreviewDiscountAmount).HasPrecision(18, 2);
        builder.Property(cart => cart.PreviewShippingAmount).HasPrecision(18, 2);
        builder.Property(cart => cart.PreviewTotalAmount).HasPrecision(18, 2);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(cart => cart.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProductListing>()
            .WithMany()
            .HasForeignKey(cart => cart.ProductListingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Campaign>()
            .WithMany()
            .HasForeignKey(cart => cart.AppliedCouponCampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(cart => cart.UserId)
            .IsUnique()
            .HasFilter("\"Status\" = 1 AND \"IsDeleted\" = false");
        builder.HasIndex(cart => cart.ProductListingId);
    }
}
