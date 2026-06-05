using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.ProductListing.DeactivateProductListing;

public sealed record DeactivateProductListingCommand(Guid Id) : ICommand<AdminProductListingDetailDto>;
