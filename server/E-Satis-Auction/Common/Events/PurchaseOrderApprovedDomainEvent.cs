using MediatR;

namespace E_Satis_Auction.Common.Events;

public sealed record PurchaseOrderApprovedDomainEvent(Guid PurchaseOrderId, string ApprovedByUserId) : INotification;
