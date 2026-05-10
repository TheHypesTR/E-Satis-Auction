using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Facility.DeleteFacility;

public record DeleteFacilityCommand(Guid Id) : IAuditableCommand;