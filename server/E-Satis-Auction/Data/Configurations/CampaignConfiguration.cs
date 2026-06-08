using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns");

        builder.Property(campaign => campaign.Version)
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Property(campaign => campaign.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(campaign => campaign.Description)
            .HasMaxLength(512);

        builder.Property(campaign => campaign.CouponCode)
            .HasMaxLength(64);

        builder.Property(campaign => campaign.Scope)
            .HasConversion<int>()
            .HasDefaultValue(CampaignScope.ProductListing)
            .IsRequired();

        builder.Property(campaign => campaign.DiscountType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(campaign => campaign.DiscountValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(campaign => campaign.MinimumOrderAmount)
            .HasPrecision(18, 2);

        builder.Property(campaign => campaign.Currency)
            .HasMaxLength(3);

        builder.Property(campaign => campaign.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasMany(campaign => campaign.Products)
            .WithOne()
            .HasForeignKey(campaignProduct => campaignProduct.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(campaign => campaign.Products)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(campaign => campaign.Status);
        builder.HasIndex(campaign => new { campaign.StartsAt, campaign.EndsAt });
        builder.HasIndex(campaign => new { campaign.Status, campaign.StartsAt, campaign.EndsAt });
        builder.HasIndex(campaign => campaign.CouponCode)
            .IsUnique()
            .HasFilter("\"CouponCode\" IS NOT NULL AND \"IsDeleted\" = false");
        builder.HasIndex(campaign => campaign.ProductListingId);
        builder.HasIndex(campaign => campaign.CategoryId);
    }
}
