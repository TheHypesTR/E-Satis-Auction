using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Category.Requests;

namespace e_Sat_Auction.Features.Category.UpdateCategoryAttributeOption;

public sealed record UpdateCategoryAttributeOptionCommand(
    Guid CategoryId, Guid AttributeId, Guid OptionId, string Value) : IAuditableCommand
{
    public UpdateCategoryAttributeOptionCommand(Guid categoryId, Guid attributeId, Guid optionId, UpdateCategoryAttributeOptionRequest request)
        : this(categoryId, attributeId, optionId, request.Value)
    {
    }
}