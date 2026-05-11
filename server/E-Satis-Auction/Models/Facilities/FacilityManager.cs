using E_Satis_Auction.Common.Entities;

namespace E_Satis_Auction.Models.Facilities;

public sealed class FacilityManager : BaseEntity
{
    public Guid FacilityId { get; private set; }
    public Facility Facility { get; private set; } = null!;
    public string UserId { get; private set; }
    public bool IsPrimary { get; private set; }

    private FacilityManager()
    {
        UserId = string.Empty;
    }

    public static FacilityManager Create(Guid facilityId, string userId, bool isPrimary = false)
    {
        return new FacilityManager
        {
            FacilityId = facilityId,
            UserId = userId,
            IsPrimary = isPrimary
        };
    }
    
    public void PromoteToPrimary()
    {
        IsPrimary = true;
    }

    public void DemoteFromPrimary()
    {
        IsPrimary = false;
    }
}