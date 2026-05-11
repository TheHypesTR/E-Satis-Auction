using E_Satis_Auction.Models.Dispatches;
using E_Satis_Auction.Models.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class DispatchLineItemConfiguration : IEntityTypeConfiguration<DispatchLineItem>
{
    public void Configure(EntityTypeBuilder<DispatchLineItem> builder)
    {
        builder.ToTable("DispatchLineItems");

        builder.Property(lineItem => lineItem.ItemNameSnapshot)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(lineItem => lineItem.DispatchId);
        builder.HasIndex(lineItem => lineItem.SourceItemId);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(lineItem => lineItem.SourceItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}