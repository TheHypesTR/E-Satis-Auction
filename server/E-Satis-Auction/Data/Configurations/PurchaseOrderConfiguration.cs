using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");

        builder.Property(order => order.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(order => order.OrderNumber)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(order => order.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(order => order.OrderSource)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(order => order.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(order => order.ShipmentStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(order => order.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(order => order.SubtotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.ApprovalNote)
            .HasMaxLength(1024);

        builder.Property(order => order.RejectionReason)
            .HasMaxLength(1024);

        builder.OwnsOne(order => order.ShippingInfo, shipping =>
        {
            shipping.Property(info => info.CarrierName)
                .HasColumnName("ShippingCarrierName")
                .HasMaxLength(128);

            shipping.Property(info => info.TrackingNumber)
                .HasColumnName("ShippingTrackingNumber")
                .HasMaxLength(128);

            shipping.Property(info => info.TrackingUrl)
                .HasColumnName("ShippingTrackingUrl")
                .HasMaxLength(512);

            shipping.Property(info => info.Notes)
                .HasColumnName("ShippingNotes")
                .HasMaxLength(1024);

            shipping.Property(info => info.ShippedAt)
                .HasColumnName("ShippedAt");
        });

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(order => order.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Lines)
            .WithOne()
            .HasForeignKey(line => line.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(order => order.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(order => order.OrderNumber)
            .IsUnique();

        builder.HasIndex(order => order.UserId);
        builder.HasIndex(order => order.Status);
        builder.HasIndex(order => order.CreatedAt);
        builder.HasIndex(order => order.OrderSource);
    }
}
