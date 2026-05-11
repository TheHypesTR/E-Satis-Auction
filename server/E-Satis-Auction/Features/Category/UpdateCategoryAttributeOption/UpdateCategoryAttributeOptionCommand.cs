using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Category.Requests;

namespace E_Satis_Auction.Features.Category.UpdateCategoryAttributeOption;

public sealed record UpdateCategoryAttributeOptionCommand(
    Guid CategoryId, Guid AttributeId, Guid OptionId, string Value) : IAuditableCommand
{
    public UpdateCategoryAttributeOptionCommand(Guid categoryId, Guid attributeId, Guid optionId, UpdateCategoryAttributeOptionRequest request)
        : this(categoryId, attributeId, optionId, request.Value)
    {
    }
}