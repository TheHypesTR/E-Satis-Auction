using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Category.DeleteCategoryAttribute;

public sealed record DeleteCategoryAttributeCommand(Guid CategoryId, Guid AttributeId) : IAuditableCommand;