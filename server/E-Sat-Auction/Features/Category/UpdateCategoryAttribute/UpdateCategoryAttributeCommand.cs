using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Category.Requests;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Category.UpdateCategoryAttribute;

public sealed record UpdateCategoryAttributeCommand(
    Guid CategoryId, Guid AttributeId, string Name, string Code, AttributeDataType DataType, AttributeTarget Target, bool IsRequired)
        : IAuditableCommand
{
    public UpdateCategoryAttributeCommand(Guid categoryId, Guid attributeId, UpdateCategoryAttributeRequest request)
        : this(categoryId, attributeId, request.Name, request.Code, request.DataType, request.Target, request.IsRequired)
    {
    }
}