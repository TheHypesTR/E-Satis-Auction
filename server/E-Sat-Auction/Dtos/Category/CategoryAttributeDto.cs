using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Dtos.Category;

public sealed record CategoryAttributeDto(
    Guid Id,
    string Name,
    string Code,
    AttributeDataType DataType,
    AttributeTarget Target,
    bool IsRequired,
    List<CategoryAttributeOptionDto> Options);