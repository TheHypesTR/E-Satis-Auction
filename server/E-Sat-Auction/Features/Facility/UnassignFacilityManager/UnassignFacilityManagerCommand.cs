using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Facility.UnassignFacilityManager;

public record UnassignFacilityManagerCommand(Guid FacilityId, string UserId) : IAuditableCommand;