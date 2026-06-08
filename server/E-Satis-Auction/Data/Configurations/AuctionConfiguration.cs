using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Products;
using E_Satis_Auction.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.ToTable("Auctions");

        builder.Property(auction => auction.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(auction => auction.StartingPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(auction => auction.CurrentPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(auction => auction.MinimumBidIncrement).HasPrecision(18, 2).IsRequired();
        builder.Property(auction => auction.WinningBidAmount).HasPrecision(18, 2);
        builder.Property(auction => auction.WaitingFeeAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(auction => auction.ServiceFeeAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(auction => auction.SellerPayoutAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(auction => auction.PlatformRevenueAmount).HasPrecision(18, 2).IsRequired();

        builder.Property(auction => auction.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(auction => auction.SellerUserId)
            .HasMaxLength(450);

        builder.Property(auction => auction.WinningUserId)
            .HasMaxLength(450);

        builder.Property(auction => auction.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne<ProductListing>()
            .WithMany()
            .HasForeignKey(auction => auction.ProductListingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(auction => auction.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(auction => auction.SellerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PurchaseOrder>()
            .WithMany()
            .HasForeignKey(auction => auction.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PaymentAttempt>()
            .WithMany()
            .HasForeignKey(auction => auction.PaymentAttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(auction => auction.Bids)
            .WithOne()
            .HasForeignKey(bid => bid.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(auction => auction.Bids)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(auction => auction.Reservations)
            .WithOne()
            .HasForeignKey(reservation => reservation.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(auction => auction.Reservations)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(auction => auction.ProductListingId);
        builder.HasIndex(auction => auction.ProductId);
        builder.HasIndex(auction => auction.Status);
        builder.HasIndex(auction => auction.StartsAt);
        builder.HasIndex(auction => auction.EndsAt);
        builder.HasIndex(auction => auction.WinningUserId);
        builder.HasIndex(auction => auction.PurchaseOrderId);
        builder.HasIndex(auction => auction.PaymentAttemptId);
    }
}
