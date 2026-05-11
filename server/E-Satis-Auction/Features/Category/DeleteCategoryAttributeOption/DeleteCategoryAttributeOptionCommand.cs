using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Category.DeleteCategoryAttributeOption;

public sealed record DeleteCategoryAttributeOptionCommand(Guid CategoryId, Guid AttributeId, Guid OptionId) : IAuditableCommand;