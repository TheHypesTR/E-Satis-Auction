using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Category.ActivateCategory;

public sealed record ActivateCategoryCommand(Guid Id) : IAuditableCommand;