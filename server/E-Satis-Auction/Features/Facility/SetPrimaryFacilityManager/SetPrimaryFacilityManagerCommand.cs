using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Facility.SetPrimaryFacilityManager;

public record SetPrimaryFacilityManagerCommand(Guid FacilityId, string UserId) : IAuditableCommand;