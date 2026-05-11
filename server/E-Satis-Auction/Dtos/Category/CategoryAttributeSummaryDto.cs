using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Category;

public sealed record CategoryAttributeSummaryDto(
    string Name,
    AttributeDataType DataType,
    AttributeTarget Target,
    bool IsRequired);