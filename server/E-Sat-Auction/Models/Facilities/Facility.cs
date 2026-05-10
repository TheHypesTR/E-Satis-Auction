using e_Sat_Auction.Common.Entities;
using e_Sat_Auction.Enums;
using e_Sat_Auction.Models.Common;

namespace e_Sat_Auction.Models.Facilities;

public class Facility : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public double CapacityM3 { get; private set; }
    public double CriticalThresholdM3 { get; private set; }
    public ApprovalStatus Status { get; private set; }

    public Guid AddressId { get; private set; }
    public Address Address { get; private set; } = null!;
    public Guid OrganizationId { get; private set; }
    public ICollection<FacilityManager> Managers { get; private set; } = new List<FacilityManager>();

    protected Facility()
    {
        Name = string.Empty;
        Description = string.Empty;
        Status = ApprovalStatus.Pending;
    }

    protected Facility(
        string name,
        string description,
        ApprovalStatus status,
        double capacityM3,
        double criticalThresholdM3,
        Guid addressId)
    {
        Name = name;
        Description = description;
        Status = status;
        CapacityM3 = capacityM3;
        CriticalThresholdM3 = criticalThresholdM3;
        AddressId = addressId;
    }
    
    public static Facility Add(
        string name,
        string description,
        ApprovalStatus status,
        double capacityM3,
        double criticalThresholdM3,
        Guid addressId)
    {
        return new Facility(
            name,
            description,
            status,
            capacityM3,
            criticalThresholdM3,
            addressId);
    }
    
    public void UpdateStatus(ApprovalStatus newStatus)
    {
        Status = newStatus;
    }
}