namespace e_Sat_Auction.Dtos.Category;

public sealed record CategoryDetailDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<CategoryAttributeDto> Attributes);