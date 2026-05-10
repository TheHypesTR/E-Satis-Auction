using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Dtos.Category.Requests;

public sealed record AddCategoryAttributeRequest(
    string Name,
    string Code,
    AttributeDataType DataType,
    AttributeTarget Target,
    bool IsRequired);