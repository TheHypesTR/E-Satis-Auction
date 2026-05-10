using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Product.DeactivateProduct;

public sealed record DeactivateProductCommand(Guid Id) : IAuditableCommand;