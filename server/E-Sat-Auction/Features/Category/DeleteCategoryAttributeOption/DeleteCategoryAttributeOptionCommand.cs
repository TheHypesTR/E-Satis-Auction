using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Category.DeleteCategoryAttributeOption;

public sealed record DeleteCategoryAttributeOptionCommand(Guid CategoryId, Guid AttributeId, Guid OptionId) : IAuditableCommand;