using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.Payment.FailPayment;

public sealed record FailPaymentCommand : ICommand<PaymentAttemptDto>
{
    public Guid PaymentAttemptId { get; }
    public FailPaymentRequest Payload { get; }

    public FailPaymentCommand(Guid paymentAttemptId, FailPaymentRequest payload)
    {
        PaymentAttemptId = paymentAttemptId;
        Payload = payload;
    }
}
