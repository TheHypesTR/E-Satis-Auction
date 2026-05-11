using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Facility.UnassignFacilityManager;

public record UnassignFacilityManagerCommand(Guid FacilityId, string UserId) : IAuditableCommand;