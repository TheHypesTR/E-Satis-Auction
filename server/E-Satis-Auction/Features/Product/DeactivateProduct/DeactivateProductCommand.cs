using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Product.DeactivateProduct;

public sealed record DeactivateProductCommand(Guid Id) : IAuditableCommand;