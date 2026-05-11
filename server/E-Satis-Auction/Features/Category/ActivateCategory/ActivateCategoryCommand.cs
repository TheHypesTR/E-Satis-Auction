using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Category.ActivateCategory;

public sealed record ActivateCategoryCommand(Guid Id) : IAuditableCommand;