using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.PurchaseOrder.BuyNow;

public sealed record BuyNowCommand(Guid ProductListingId, int Quantity, Guid? CampaignId, string? IdempotencyKey) : IAuditableCommand<OrderDetailDto>;
