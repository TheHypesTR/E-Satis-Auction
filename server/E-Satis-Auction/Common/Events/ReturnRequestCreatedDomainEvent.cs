using MediatR;

namespace E_Satis_Auction.Common.Events;

public sealed record ReturnRequestCreatedDomainEvent(Guid ReturnRequestId, Guid PurchaseOrderId, string UserId) : INotification;
