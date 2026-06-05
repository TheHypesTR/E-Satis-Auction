using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.ProductListing.UpdateProductListing;

public sealed record UpdateProductListingCommand(
    Guid Id,
    decimal Price,
    string Currency,
    DateTimeOffset? ActiveFrom,
    DateTimeOffset? ActiveUntil) : ICommand<AdminProductListingDetailDto>
{
    public UpdateProductListingCommand(Guid id, UpdateProductListingRequest request)
        : this(id, request.Price, request.Currency, request.ActiveFrom, request.ActiveUntil)
    {
    }
}
