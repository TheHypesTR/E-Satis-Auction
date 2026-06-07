using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.ProductListing.GetAdminProductListingById;

public sealed record GetAdminProductListingByIdQuery(Guid Id) : IQuery<AdminProductListingDetailDto>;
