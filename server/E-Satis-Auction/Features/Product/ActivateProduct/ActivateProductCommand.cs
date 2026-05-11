using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Product.ActivateProduct;

public sealed record ActivateProductCommand(Guid Id) : IAuditableCommand;