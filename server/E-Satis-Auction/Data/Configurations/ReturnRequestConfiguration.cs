using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
{
    public void Configure(EntityTypeBuilder<ReturnRequest> builder)
    {
        builder.ToTable("ReturnRequests");

        builder.Property(returnRequest => returnRequest.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(returnRequest => returnRequest.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(returnRequest => returnRequest.Reason)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(returnRequest => returnRequest.ResolutionNote)
            .HasMaxLength(1024);

        builder.Property(returnRequest => returnRequest.ReceivedByUserId)
            .HasMaxLength(450);

        builder.Property(returnRequest => returnRequest.ReceiveNote)
            .HasMaxLength(1024);

        builder.HasOne<PurchaseOrder>()
            .WithMany()
            .HasForeignKey(returnRequest => returnRequest.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(returnRequest => returnRequest.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(returnRequest => returnRequest.ReceivedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(returnRequest => returnRequest.Lines)
            .WithOne()
            .HasForeignKey(line => line.ReturnRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(returnRequest => returnRequest.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(returnRequest => returnRequest.PurchaseOrderId);
        builder.HasIndex(returnRequest => returnRequest.UserId);
        builder.HasIndex(returnRequest => returnRequest.Status);
        builder.HasIndex(returnRequest => returnRequest.ReceivedAt);
    }
}
