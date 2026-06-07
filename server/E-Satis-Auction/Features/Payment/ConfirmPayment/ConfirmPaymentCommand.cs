using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.Payment.ConfirmPayment;

public sealed record ConfirmPaymentCommand : ICommand<PaymentAttemptDto>
{
    public Guid PaymentAttemptId { get; }
    public ConfirmPaymentRequest Payload { get; }

    public ConfirmPaymentCommand(Guid paymentAttemptId, ConfirmPaymentRequest payload)
    {
        PaymentAttemptId = paymentAttemptId;
        Payload = payload;
    }
}
