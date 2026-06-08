using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Auction.Requests;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.AdminAuction.CreateAuction;

using AuctionEntity = Models.Commerce.Auction;
using ProductListingEntity = Models.Commerce.ProductListing;
using ProductEntity = Models.Products.Product;

public sealed class CreateAuctionCommandHandler : ICommandHandler<CreateAuctionCommand, AuctionDetailDto>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IProductListingRepository _productListingRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAuctionCommandHandler(
        IAuctionRepository auctionRepository,
        IProductListingRepository productListingRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _auctionRepository = auctionRepository;
        _productListingRepository = productListingRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuctionDetailDto> Handle(CreateAuctionCommand command, CancellationToken cancellationToken)
    {
        CreateAuctionRequest payload = command.Payload;

        ProductListingEntity? listing = await _productListingRepository.GetByIdAsync(payload.ProductListingId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(listing, ErrorMessages.ProductListing.EntityName, payload.ProductListingId);

        BusinessException.ThrowIfTrue(
            await _auctionRepository.HasOpenAuctionForProductListingAsync(payload.ProductListingId, cancellationToken: cancellationToken),
            ErrorMessages.Auction.ProductListingAlreadyInAuction,
            ErrorMessages.Exception.CommerceTitle);

        ProductEntity? product = await _productRepository.GetByIdAsync(listing!.ProductId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, listing.ProductId);

        BusinessException.ThrowIfFalse(product!.IsActive, ErrorMessages.Product.ProductNotAvailable, ErrorMessages.Exception.ProductTitle);
        BusinessException.ThrowIfFalse(string.Equals(listing.Currency, payload.Currency, StringComparison.OrdinalIgnoreCase), ErrorMessages.PurchaseOrder.CurrencyMismatch, ErrorMessages.Exception.CommerceTitle);

        AuctionEntity auction = AuctionEntity.Create(
            listing.Id,
            listing.ProductId,
            payload.SellerUserId,
            payload.StartingPrice,
            payload.MinimumBidIncrement,
            payload.StartsAt,
            payload.EndsAt,
            payload.Quantity,
            payload.Currency);

        await _auctionRepository.AddAsync(auction, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        Dictionary<Guid, ProductListingProductEnrichmentDto> products = await _productRepository.GetProductListingEnrichmentsByIdsAsync([auction.ProductId], cancellationToken);
        return AuctionDtoMapper.ToDetailDto(auction, products[auction.ProductId]);
    }
}
