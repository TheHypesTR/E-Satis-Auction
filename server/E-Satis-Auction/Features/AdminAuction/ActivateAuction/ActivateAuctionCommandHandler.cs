using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.AdminAuction.ActivateAuction;

public sealed class ActivateAuctionCommandHandler : ICommandHandler<ActivateAuctionCommand, AuctionDetailDto>
{
    private readonly IAuctionWorkflowService _auctionWorkflowService;

    public ActivateAuctionCommandHandler(IAuctionWorkflowService auctionWorkflowService)
    {
        _auctionWorkflowService = auctionWorkflowService;
    }

    public async Task<AuctionDetailDto> Handle(ActivateAuctionCommand command, CancellationToken cancellationToken)
    {
        return await _auctionWorkflowService.ActivateAuctionAsync(command.AuctionId, cancellationToken);
    }
}
