using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Services;

public interface ICommerceWorkflowService
{
    Task<CartPricePreviewDto> PreviewCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default);
    Task<PaymentInitiationDto> InitiatePaymentFromCartAsync(string userId, string idempotencyKey, CancellationToken cancellationToken = default);
    Task<PaymentAttemptDto> ConfirmPaymentAsync(string userId, Guid paymentAttemptId, string idempotencyKey, CancellationToken cancellationToken = default);
    Task<PaymentAttemptDto> FailPaymentAsync(string userId, Guid paymentAttemptId, string idempotencyKey, string reason, CancellationToken cancellationToken = default);
    Task ExpirePaymentAsync(PaymentAttempt paymentAttempt, CancellationToken cancellationToken = default);
}
