using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.Payment.ConfirmPayment;

public sealed class ConfirmPaymentCommandHandler : ICommandHandler<ConfirmPaymentCommand, PaymentAttemptDto>
{
    private readonly ICommerceWorkflowService _commerceWorkflowService;
    private readonly ICurrentUserService _currentUserService;

    public ConfirmPaymentCommandHandler(ICommerceWorkflowService commerceWorkflowService, ICurrentUserService currentUserService)
    {
        _commerceWorkflowService = commerceWorkflowService;
        _currentUserService = currentUserService;
    }

    public async Task<PaymentAttemptDto> Handle(ConfirmPaymentCommand command, CancellationToken cancellationToken)
    {
        return await _commerceWorkflowService.ConfirmPaymentAsync(_currentUserService.UserId, command.PaymentAttemptId, command.Payload.IdempotencyKey, cancellationToken);
    }
}
