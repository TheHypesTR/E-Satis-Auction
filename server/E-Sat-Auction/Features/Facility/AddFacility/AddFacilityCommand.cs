using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Facility.AddFacility;

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