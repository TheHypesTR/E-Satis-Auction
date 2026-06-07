namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record FailPaymentRequest(string IdempotencyKey, string Reason);
