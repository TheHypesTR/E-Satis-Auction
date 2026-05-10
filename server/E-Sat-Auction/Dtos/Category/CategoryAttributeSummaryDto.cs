using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Dtos.Category;

public sealed record CategoryAttributeSummaryDto(
    string Name,
    AttributeDataType DataType,
    AttributeTarget Target,
    bool IsRequired);