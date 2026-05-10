using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Category.DeactivateCategory;

public sealed record DeactivateCategoryCommand(Guid Id) : IAuditableCommand;