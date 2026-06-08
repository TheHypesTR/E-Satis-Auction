using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.ToTable("PaymentAttempts");

        builder.Property(payment => payment.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(payment => payment.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(payment => payment.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(payment => payment.IdempotencyKey)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(payment => payment.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(payment => payment.FailureReason)
            .HasMaxLength(1024);

        builder.HasOne<PurchaseOrder>()
            .WithMany()
            .HasForeignKey(payment => payment.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(payment => payment.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(payment => payment.IdempotencyKey)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
        builder.HasIndex(payment => payment.PurchaseOrderId);
        builder.HasIndex(payment => new { payment.Status, payment.ExpiresAt });
    }
}
