using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.Payment.GetPaymentAttempt;

public sealed record GetPaymentAttemptQuery(Guid PaymentAttemptId) : IQuery<PaymentAttemptDto>;
