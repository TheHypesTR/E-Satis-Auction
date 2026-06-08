using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.PurchaseOrder.GetMyOrderById;

public sealed record GetMyOrderByIdQuery(Guid PurchaseOrderId) : IQuery<OrderDetailDto>;
