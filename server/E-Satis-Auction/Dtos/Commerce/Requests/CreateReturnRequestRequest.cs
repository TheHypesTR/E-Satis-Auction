namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record CreateReturnRequestRequest(string Reason, IReadOnlyCollection<CreateReturnRequestLineRequest> Lines);
