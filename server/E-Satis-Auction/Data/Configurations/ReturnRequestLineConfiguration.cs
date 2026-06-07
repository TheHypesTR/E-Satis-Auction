using E_Satis_Auction.Models.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class ReturnRequestLineConfiguration : IEntityTypeConfiguration<ReturnRequestLine>
{
    public void Configure(EntityTypeBuilder<ReturnRequestLine> builder)
    {
        builder.ToTable("ReturnRequestLines");

        builder.Property(line => line.Reason)
            .HasMaxLength(1024);

        builder.Property(line => line.ReceiveNote)
            .HasMaxLength(1024);

        builder.HasOne<PurchaseOrderLine>()
            .WithMany()
            .HasForeignKey(line => line.PurchaseOrderLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(line => line.ReturnRequestId);
        builder.HasIndex(line => line.PurchaseOrderLineId);
    }
}
