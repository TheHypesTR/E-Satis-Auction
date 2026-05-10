using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Category.Requests;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Category.AddCategoryAttribute;

public sealed record AddCategoryAttributeCommand(
    Guid CategoryId, string Name, string Code, AttributeDataType DataType, AttributeTarget Target, bool IsRequired) : IAuditableCommand<Guid>
{
    public AddCategoryAttributeCommand(Guid categoryId, AddCategoryAttributeRequest request)
        : this(categoryId, request.Name, request.Code, request.DataType, request.Target, request.IsRequired)
    {
    }
}