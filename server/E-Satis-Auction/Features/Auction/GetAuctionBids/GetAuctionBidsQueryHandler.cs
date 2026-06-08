using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Auction.GetAuctionBids;

using AuctionBidEntity = Models.Commerce.AuctionBid;

public sealed class GetAuctionBidsQueryHandler : IQueryHandler<GetAuctionBidsQuery, PaginatedList<AuctionBidDto>>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IAuctionBidRepository _auctionBidRepository;

    public GetAuctionBidsQueryHandler(IAuctionRepository auctionRepository, IAuctionBidRepository auctionBidRepository)
    {
        _auctionRepository = auctionRepository;
        _auctionBidRepository = auctionBidRepository;
    }

    public async Task<PaginatedList<AuctionBidDto>> Handle(GetAuctionBidsQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.Auction? auction = await _auctionRepository.GetByIdAsync(query.AuctionId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(auction, ErrorMessages.Auction.EntityName, query.AuctionId);

        PaginatedList<AuctionBidEntity> bids = await _auctionBidRepository.GetAcceptedBidsPaginatedAsync(
            query.AuctionId,
            auction!.StartsAt.UtcDateTime,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        return new PaginatedList<AuctionBidDto>(
            bids.Items.Select(AuctionDtoMapper.ToBidDto).ToList(),
            bids.TotalCount,
            bids.PageNumber,
            query.PageSize);
    }
}
