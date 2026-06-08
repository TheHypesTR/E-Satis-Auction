using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.Payment.InitiatePayment;

public sealed class InitiatePaymentCommandHandler : ICommandHandler<InitiatePaymentCommand, PaymentInitiationDto>
{
    private readonly ICommerceWorkflowService _commerceWorkflowService;
    private readonly ICurrentUserService _currentUserService;

    public InitiatePaymentCommandHandler(ICommerceWorkflowService commerceWorkflowService, ICurrentUserService currentUserService)
    {
        _commerceWorkflowService = commerceWorkflowService;
        _currentUserService = currentUserService;
    }

    public async Task<PaymentInitiationDto> Handle(InitiatePaymentCommand command, CancellationToken cancellationToken)
    {
        return await _commerceWorkflowService.InitiatePaymentFromCartAsync(_currentUserService.UserId, command.Payload.IdempotencyKey, cancellationToken);
    }
}
