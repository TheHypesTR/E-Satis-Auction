using MediatR;

namespace E_Satis_Auction.Common.Events;

public sealed record PurchaseOrderRejectedDomainEvent(Guid PurchaseOrderId, string RejectedByUserId) : INotification;
