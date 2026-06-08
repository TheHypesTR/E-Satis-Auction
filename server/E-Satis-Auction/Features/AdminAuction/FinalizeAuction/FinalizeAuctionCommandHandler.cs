using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.AdminAuction.FinalizeAuction;

public sealed class FinalizeAuctionCommandHandler : ICommandHandler<FinalizeAuctionCommand, AuctionDetailDto>
{
    private readonly IAuctionWorkflowService _auctionWorkflowService;

    public FinalizeAuctionCommandHandler(IAuctionWorkflowService auctionWorkflowService)
    {
        _auctionWorkflowService = auctionWorkflowService;
    }

    public async Task<AuctionDetailDto> Handle(FinalizeAuctionCommand command, CancellationToken cancellationToken)
    {
        return await _auctionWorkflowService.FinalizeAuctionAsync(command.AuctionId, cancellationToken);
    }
}
