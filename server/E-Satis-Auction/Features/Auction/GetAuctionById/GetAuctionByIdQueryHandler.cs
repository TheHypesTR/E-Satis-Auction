using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Auction.GetAuctionById;

public sealed class GetAuctionByIdQueryHandler : IQueryHandler<GetAuctionByIdQuery, AuctionDetailDto>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IProductRepository _productRepository;

    public GetAuctionByIdQueryHandler(IAuctionRepository auctionRepository, IProductRepository productRepository)
    {
        _auctionRepository = auctionRepository;
        _productRepository = productRepository;
    }

    public async Task<AuctionDetailDto> Handle(GetAuctionByIdQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.Auction? auction = await _auctionRepository.GetByIdWithDetailsAsync(query.AuctionId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(auction, ErrorMessages.Auction.EntityName, query.AuctionId);

        Dictionary<Guid, ProductListingProductEnrichmentDto> products = await _productRepository.GetProductListingEnrichmentsByIdsAsync([auction!.ProductId], cancellationToken);
        return AuctionDtoMapper.ToDetailDto(auction, products[auction.ProductId]);
    }
}
