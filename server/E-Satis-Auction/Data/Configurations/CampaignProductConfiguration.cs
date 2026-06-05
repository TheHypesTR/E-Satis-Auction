using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public sealed class CampaignProductConfiguration : IEntityTypeConfiguration<CampaignProduct>
{
    public void Configure(EntityTypeBuilder<CampaignProduct> builder)
    {
        builder.ToTable("CampaignProducts");

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(campaignProduct => campaignProduct.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(campaignProduct => campaignProduct.CampaignId);
        builder.HasIndex(campaignProduct => campaignProduct.ProductId);
        builder.HasIndex(campaignProduct => new { campaignProduct.CampaignId, campaignProduct.ProductId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
