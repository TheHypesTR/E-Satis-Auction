using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.ProductListing.CreateProductListing;

public sealed record CreateProductListingCommand(
    Guid ProductId,
    Guid SourceFacilityId,
    decimal Price,
    string Currency,
    DateTimeOffset? ActiveFrom,
    DateTimeOffset? ActiveUntil) : ICommand<AdminProductListingDetailDto>
{
    public CreateProductListingCommand(CreateProductListingRequest request)
        : this(request.ProductId, request.SourceFacilityId, request.Price, request.Currency, request.ActiveFrom, request.ActiveUntil)
    {
    }
}
