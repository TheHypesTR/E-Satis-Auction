using MediatR;

namespace E_Satis_Auction.Common.Events;

public sealed record PurchaseOrderCreatedDomainEvent(Guid PurchaseOrderId, string UserId) : INotification;
