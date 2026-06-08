using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.AdminAuction.CancelAuction;

public sealed class CancelAuctionCommandHandler : ICommandHandler<CancelAuctionCommand, AuctionDetailDto>
{
    private readonly IAuctionWorkflowService _auctionWorkflowService;

    public CancelAuctionCommandHandler(IAuctionWorkflowService auctionWorkflowService)
    {
        _auctionWorkflowService = auctionWorkflowService;
    }

    public async Task<AuctionDetailDto> Handle(CancelAuctionCommand command, CancellationToken cancellationToken)
    {
        return await _auctionWorkflowService.CancelAuctionAsync(command.AuctionId, cancellationToken);
    }
}
