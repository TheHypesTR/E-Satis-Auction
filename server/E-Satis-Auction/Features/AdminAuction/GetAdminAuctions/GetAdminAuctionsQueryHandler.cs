using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.AdminAuction.GetAdminAuctions;

using AuctionEntity = Models.Commerce.Auction;

public sealed class GetAdminAuctionsQueryHandler : IQueryHandler<GetAdminAuctionsQuery, PaginatedList<AuctionSummaryDto>>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IProductRepository _productRepository;

    public GetAdminAuctionsQueryHandler(IAuctionRepository auctionRepository, IProductRepository productRepository)
    {
        _auctionRepository = auctionRepository;
        _productRepository = productRepository;
    }

    public async Task<PaginatedList<AuctionSummaryDto>> Handle(GetAdminAuctionsQuery query, CancellationToken cancellationToken)
    {
        PaginatedList<AuctionEntity> auctions = await _auctionRepository.GetAdminAuctionsPaginatedAsync(
            query.Status,
            query.ProductListingId,
            query.ProductId,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        return await MapAsync(auctions, query.PageSize, cancellationToken);
    }

    private async Task<PaginatedList<AuctionSummaryDto>> MapAsync(PaginatedList<AuctionEntity> auctions, int pageSize, CancellationToken cancellationToken)
    {
        if (auctions.Items.Count is 0)
        {
            return new PaginatedList<AuctionSummaryDto>([], auctions.TotalCount, auctions.PageNumber, pageSize);
        }

        Dictionary<Guid, ProductListingProductEnrichmentDto> products = await _productRepository
            .GetProductListingEnrichmentsByIdsAsync(auctions.Items.Select(auction => auction.ProductId), cancellationToken);

        List<AuctionSummaryDto> items = auctions.Items
            .Where(auction => products.ContainsKey(auction.ProductId))
            .Select(auction => AuctionDtoMapper.ToSummaryDto(auction, products[auction.ProductId]))
            .ToList();

        return new PaginatedList<AuctionSummaryDto>(items, auctions.TotalCount, auctions.PageNumber, pageSize);
    }
}
