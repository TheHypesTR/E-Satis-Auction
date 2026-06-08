namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record ApproveUserSaleRequestRequest(decimal AcquisitionPrice, decimal TargetResalePrice, string? AdminNote);
