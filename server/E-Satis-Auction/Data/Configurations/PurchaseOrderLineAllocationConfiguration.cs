using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class PurchaseOrderLineAllocationConfiguration : IEntityTypeConfiguration<PurchaseOrderLineAllocation>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLineAllocation> builder)
    {
        builder.ToTable("PurchaseOrderLineAllocations");

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(allocation => allocation.OriginalItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(allocation => allocation.ReservedItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(allocation => allocation.PurchaseOrderLineId);
        builder.HasIndex(allocation => allocation.OriginalItemId);
        builder.HasIndex(allocation => allocation.ReservedItemId);
    }
}
