using E_Satis_Auction.Models.Facilities;
using E_Satis_Auction.Models.InventoryTransactions;
using E_Satis_Auction.Models.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("InventoryTransactions");

        builder.Property(transaction => transaction.TransactionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(transaction => transaction.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(transaction => transaction.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Facility>()
            .WithMany()
            .HasForeignKey(transaction => transaction.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(transaction => transaction.ItemId);
        builder.HasIndex(transaction => transaction.FacilityId);
        builder.HasIndex(transaction => transaction.CreatedAt);
    }
}