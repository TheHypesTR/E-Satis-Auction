using e_Sat_Auction.Models.Common;
using e_Sat_Auction.Models.Dispatches;
using e_Sat_Auction.Models.Facilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace e_Sat_Auction.Data.Configurations;

public sealed class DispatchConfiguration : IEntityTypeConfiguration<Dispatch>
{
    public void Configure(EntityTypeBuilder<Dispatch> builder)
    {
        builder.ToTable("Dispatches");

        builder.Property(dispatch => dispatch.TrackingNumber)
            .HasMaxLength(32)
            .IsRequired();
        
        builder.Property(dispatch => dispatch.ReceiverName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(dispatch => dispatch.ReceiverPhone)
            .HasMaxLength(32)
            .IsRequired();
        
        builder.Property(dispatch => dispatch.Notes)
            .HasMaxLength(1024);
        
        builder.Property(x => x.DeliveryNote)
            .HasMaxLength(1024);

        builder.Property(dispatch => dispatch.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(dispatch => dispatch.SourceFacilityId);
        builder.HasIndex(dispatch => dispatch.TargetFacilityId);
        builder.HasIndex(dispatch => dispatch.TargetAddressId);
        builder.HasIndex(dispatch => dispatch.Status);
        builder.HasIndex(dispatch => dispatch.TrackingNumber)
            .IsUnique();

        builder.HasOne<Facility>()
            .WithMany()
            .HasForeignKey(dispatch => dispatch.SourceFacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Facility>()
            .WithMany()
            .HasForeignKey(dispatch => dispatch.TargetFacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Address>()
            .WithMany()
            .HasForeignKey(dispatch => dispatch.TargetAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(dispatch => dispatch.LineItems)
            .WithOne()
            .HasForeignKey(lineItem => lineItem.DispatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(dispatch => dispatch.LineItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}