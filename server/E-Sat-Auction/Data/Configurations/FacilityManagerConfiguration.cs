using e_Sat_Auction.Models.Facilities;
using e_Sat_Auction.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_Sat_Auction.Data.Configurations;

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