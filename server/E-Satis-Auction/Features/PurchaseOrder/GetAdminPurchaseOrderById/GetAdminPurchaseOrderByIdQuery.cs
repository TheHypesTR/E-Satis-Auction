using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.PurchaseOrder.GetAdminPurchaseOrderById;

public sealed record GetAdminPurchaseOrderByIdQuery(Guid PurchaseOrderId) : IQuery<AdminOrderDetailDto>;
