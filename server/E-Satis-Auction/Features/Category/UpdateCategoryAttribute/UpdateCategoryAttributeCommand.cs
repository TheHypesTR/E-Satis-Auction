using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Category.Requests;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Category.UpdateCategoryAttribute;

public sealed record UpdateCategoryAttributeCommand(
    Guid CategoryId, Guid AttributeId, string Name, string Code, AttributeDataType DataType, AttributeTarget Target, bool IsRequired)
        : IAuditableCommand
{
    public UpdateCategoryAttributeCommand(Guid categoryId, Guid attributeId, UpdateCategoryAttributeRequest request)
        : this(categoryId, attributeId, request.Name, request.Code, request.DataType, request.Target, request.IsRequired)
    {
    }
}