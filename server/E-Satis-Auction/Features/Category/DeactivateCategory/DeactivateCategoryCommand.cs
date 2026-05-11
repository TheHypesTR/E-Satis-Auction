using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Category.DeactivateCategory;

public sealed record DeactivateCategoryCommand(Guid Id) : IAuditableCommand;