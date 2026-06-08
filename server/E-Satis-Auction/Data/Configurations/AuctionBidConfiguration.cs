using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class AuctionBidConfiguration : IEntityTypeConfiguration<AuctionBid>
{
    public void Configure(EntityTypeBuilder<AuctionBid> builder)
    {
        builder.ToTable("AuctionBids");

        builder.Property(bid => bid.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(bid => bid.BidderUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(bid => bid.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(bid => bid.IdempotencyKey)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(bid => bid.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne<Auction>()
            .WithMany(auction => auction.Bids)
            .HasForeignKey(bid => bid.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(bid => bid.BidderUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(bid => bid.AuctionId);
        builder.HasIndex(bid => bid.BidderUserId);
        builder.HasIndex(bid => new { bid.AuctionId, bid.BidderUserId, bid.IdempotencyKey })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
