using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Auction.GetAuctionWinner;

public sealed class GetAuctionWinnerQueryHandler : IQueryHandler<GetAuctionWinnerQuery, AuctionWinnerDto>
{
    private readonly IAuctionRepository _auctionRepository;

    public GetAuctionWinnerQueryHandler(IAuctionRepository auctionRepository)
    {
        _auctionRepository = auctionRepository;
    }

    public async Task<AuctionWinnerDto> Handle(GetAuctionWinnerQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.Auction? auction = await _auctionRepository.GetByIdAsync(query.AuctionId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(auction, ErrorMessages.Auction.EntityName, query.AuctionId);

        return AuctionDtoMapper.ToWinnerDto(auction!);
    }
}
