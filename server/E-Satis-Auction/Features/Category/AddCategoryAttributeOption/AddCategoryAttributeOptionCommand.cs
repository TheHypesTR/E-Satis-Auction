using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Category.Requests;

namespace E_Satis_Auction.Features.Category.AddCategoryAttributeOption;

public sealed record AddCategoryAttributeOptionCommand(Guid CategoryId, Guid AttributeId, string Value) : IAuditableCommand<Guid>
{
    public AddCategoryAttributeOptionCommand(Guid categoryId, Guid attributeId, AddCategoryAttributeOptionRequest request)
        : this(categoryId, attributeId, request.Value)
    {
    }
}