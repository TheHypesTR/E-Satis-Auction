using E_Satis_Auction.Models.Categories;
using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class UserSaleRequestConfiguration : IEntityTypeConfiguration<UserSaleRequest>
{
    public void Configure(EntityTypeBuilder<UserSaleRequest> builder)
    {
        builder.ToTable("UserSaleRequests");

        builder.Property(request => request.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(request => request.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(request => request.Title)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(request => request.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(request => request.UserEstimatedValue).HasPrecision(18, 2);
        builder.Property(request => request.AcquisitionPrice).HasPrecision(18, 2);
        builder.Property(request => request.TargetResalePrice).HasPrecision(18, 2);
        builder.Property(request => request.ExpectedProfit).HasPrecision(18, 2);

        builder.Property(request => request.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(request => request.AdminNote)
            .HasMaxLength(1024);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(request => request.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(request => request.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(request => request.UserId);
        builder.HasIndex(request => request.Status);
        builder.HasIndex(request => request.CreatedAt);
    }
}
