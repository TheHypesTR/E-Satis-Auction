using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Category.Requests;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Category.AddCategoryAttribute;

public sealed record AddCategoryAttributeCommand(
    Guid CategoryId, string Name, string Code, AttributeDataType DataType, AttributeTarget Target, bool IsRequired) : IAuditableCommand<Guid>
{
    public AddCategoryAttributeCommand(Guid categoryId, AddCategoryAttributeRequest request)
        : this(categoryId, request.Name, request.Code, request.DataType, request.Target, request.IsRequired)
    {
    }
}