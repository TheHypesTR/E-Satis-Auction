using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class AuctionInventoryReservationConfiguration : IEntityTypeConfiguration<AuctionInventoryReservation>
{
    public void Configure(EntityTypeBuilder<AuctionInventoryReservation> builder)
    {
        builder.ToTable("AuctionInventoryReservations");

        builder.Property(reservation => reservation.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne<Auction>()
            .WithMany(auction => auction.Reservations)
            .HasForeignKey(reservation => reservation.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(reservation => reservation.OriginalItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(reservation => reservation.ReservedItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(reservation => reservation.AuctionId);
        builder.HasIndex(reservation => reservation.OriginalItemId);
        builder.HasIndex(reservation => reservation.ReservedItemId);
        builder.HasIndex(reservation => reservation.Status);
    }
}
