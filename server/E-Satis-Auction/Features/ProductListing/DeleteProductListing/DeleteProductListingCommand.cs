using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.ProductListing.DeleteProductListing;

public sealed record DeleteProductListingCommand(Guid Id) : ICommand;
