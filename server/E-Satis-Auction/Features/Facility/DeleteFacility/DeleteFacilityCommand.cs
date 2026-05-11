using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Facility.DeleteFacility;

public record DeleteFacilityCommand(Guid Id) : IAuditableCommand;