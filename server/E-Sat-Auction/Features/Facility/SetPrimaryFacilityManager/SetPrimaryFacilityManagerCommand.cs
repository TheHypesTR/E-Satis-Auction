using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Facility.SetPrimaryFacilityManager;

public record SetPrimaryFacilityManagerCommand(Guid FacilityId, string UserId) : IAuditableCommand;