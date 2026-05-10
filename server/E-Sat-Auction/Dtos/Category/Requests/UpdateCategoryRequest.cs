namespace e_Sat_Auction.Dtos.Category.Requests;

public sealed record UpdateCategoryRequest(
    string Name,
    string? Description);