using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.AdminAuction.UpdateAuction;

using AuctionEntity = Models.Commerce.Auction;

public sealed class UpdateAuctionCommandHandler : ICommandHandler<UpdateAuctionCommand, AuctionDetailDto>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAuctionCommandHandler(IAuctionRepository auctionRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _auctionRepository = auctionRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuctionDetailDto> Handle(UpdateAuctionCommand command, CancellationToken cancellationToken)
    {
        AuctionEntity? auction = await _auctionRepository.GetByIdWithDetailsAsync(command.AuctionId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(auction, ErrorMessages.Auction.EntityName, command.AuctionId);

        auction!.Update(
            command.Payload.StartingPrice,
            command.Payload.MinimumBidIncrement,
            command.Payload.StartsAt,
            command.Payload.EndsAt,
            command.Payload.Quantity,
            command.Payload.Currency,
            command.Payload.SellerUserId);

        _auctionRepository.Update(auction);
        await _unitOfWork.CompleteAsync(cancellationToken);

        Dictionary<Guid, ProductListingProductEnrichmentDto> products = await _productRepository.GetProductListingEnrichmentsByIdsAsync([auction.ProductId], cancellationToken);
        return AuctionDtoMapper.ToDetailDto(auction, products[auction.ProductId]);
    }
}
