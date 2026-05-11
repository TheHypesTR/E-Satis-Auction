using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Category;

public sealed record CategoryAttributeDto(
    Guid Id,
    string Name,
    string Code,
    AttributeDataType DataType,
    AttributeTarget Target,
    bool IsRequired,
    List<CategoryAttributeOptionDto> Options);