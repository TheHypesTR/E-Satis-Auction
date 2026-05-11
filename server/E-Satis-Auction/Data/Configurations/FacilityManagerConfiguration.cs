using E_Satis_Auction.Models.Facilities;
using E_Satis_Auction.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Satis_Auction.Data.Configurations;

public class FacilityManagerConfiguration : IEntityTypeConfiguration<FacilityManager>
{
    public void Configure(EntityTypeBuilder<FacilityManager> builder)
    {
        builder.HasOne(fm => fm.Facility)
            .WithMany(f => f.Managers)
            .HasForeignKey(fm => fm.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(fm => fm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(fm => fm.UserId);
    }
}