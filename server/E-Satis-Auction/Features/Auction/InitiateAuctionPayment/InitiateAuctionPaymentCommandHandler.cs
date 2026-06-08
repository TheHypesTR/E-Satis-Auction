using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.Auction.InitiateAuctionPayment;

public sealed class InitiateAuctionPaymentCommandHandler : ICommandHandler<InitiateAuctionPaymentCommand, PaymentInitiationDto>
{
    private readonly IAuctionWorkflowService _auctionWorkflowService;
    private readonly ICurrentUserService _currentUserService;

    public InitiateAuctionPaymentCommandHandler(IAuctionWorkflowService auctionWorkflowService, ICurrentUserService currentUserService)
    {
        _auctionWorkflowService = auctionWorkflowService;
        _currentUserService = currentUserService;
    }

    public async Task<PaymentInitiationDto> Handle(InitiateAuctionPaymentCommand command, CancellationToken cancellationToken)
    {
        return await _auctionWorkflowService.InitiateWinnerPaymentAsync(
            command.AuctionId,
            _currentUserService.UserId,
            command.Payload.IdempotencyKey,
            cancellationToken);
    }
}
