namespace e_Sat_Auction.Dtos.Category;

public sealed record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<CategoryAttributeSummaryDto> Attributes);