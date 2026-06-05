using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.ProductListing.GetProductListingById;

public sealed record GetProductListingByIdQuery(Guid Id) : IQuery<ProductListingDetailDto>;
