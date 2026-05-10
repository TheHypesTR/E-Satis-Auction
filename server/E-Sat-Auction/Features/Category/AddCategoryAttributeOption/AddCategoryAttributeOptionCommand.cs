using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Category.Requests;

namespace e_Sat_Auction.Features.Category.AddCategoryAttributeOption;

public sealed record AddCategoryAttributeOptionCommand(Guid CategoryId, Guid AttributeId, string Value) : IAuditableCommand<Guid>
{
    public AddCategoryAttributeOptionCommand(Guid categoryId, Guid attributeId, AddCategoryAttributeOptionRequest request)
        : this(categoryId, attributeId, request.Value)
    {
    }
}