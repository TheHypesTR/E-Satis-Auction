using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Product.ActivateProduct;

public sealed record ActivateProductCommand(Guid Id) : IAuditableCommand;