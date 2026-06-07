namespace E_Satis_Auction.Dtos.Commerce;

public sealed record PaymentInitiationDto(PaymentAttemptDto Payment, OrderDetailDto Order);
