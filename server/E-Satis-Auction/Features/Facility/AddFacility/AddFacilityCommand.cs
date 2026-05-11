using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Facility.AddFacility;

public record AddFacilityCommand(
    string Name,
    string Description,
    bool IsVisibleOnMap,
    double CapacityM3,
    double CriticalThresholdM3,
    Guid OrganizationId,
    string AddressTitle,
    string City,
    string District,
    string OpenAddress,
    double Latitude,
    double Longitude) : IAuditableCommand<Guid>;