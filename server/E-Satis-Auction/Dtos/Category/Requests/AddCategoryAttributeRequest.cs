using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Category.Requests;

public sealed record AddCategoryAttributeRequest(
    string Name,
    string Code,
    AttributeDataType DataType,
    AttributeTarget Target,
    bool IsRequired);