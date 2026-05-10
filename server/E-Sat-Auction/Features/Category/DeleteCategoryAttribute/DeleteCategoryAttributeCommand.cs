using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Category.DeleteCategoryAttribute;

public sealed record DeleteCategoryAttributeCommand(Guid CategoryId, Guid AttributeId) : IAuditableCommand;