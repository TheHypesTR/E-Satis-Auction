namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record CreateUserSaleRequestRequest(
    string Title,
    string Description,
    Guid CategoryId,
    decimal UserEstimatedValue);
