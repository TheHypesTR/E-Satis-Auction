using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.Payment.FailPayment;

public sealed class FailPaymentCommandHandler : ICommandHandler<FailPaymentCommand, PaymentAttemptDto>
{
    private readonly ICommerceWorkflowService _commerceWorkflowService;
    private readonly ICurrentUserService _currentUserService;

    public FailPaymentCommandHandler(ICommerceWorkflowService commerceWorkflowService, ICurrentUserService currentUserService)
    {
        _commerceWorkflowService = commerceWorkflowService;
        _currentUserService = currentUserService;
    }

    public async Task<PaymentAttemptDto> Handle(FailPaymentCommand command, CancellationToken cancellationToken)
    {
        return await _commerceWorkflowService.FailPaymentAsync(_currentUserService.UserId, command.PaymentAttemptId, command.Payload.IdempotencyKey, command.Payload.Reason, cancellationToken);
    }
}
