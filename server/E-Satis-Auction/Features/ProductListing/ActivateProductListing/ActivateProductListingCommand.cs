using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.ProductListing.ActivateProductListing;

public sealed record ActivateProductListingCommand(Guid Id) : ICommand<AdminProductListingDetailDto>;
