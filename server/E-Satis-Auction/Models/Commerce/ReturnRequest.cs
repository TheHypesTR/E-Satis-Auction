using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Events;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class ReturnRequest : BaseEntity
{
    public Guid PurchaseOrderId { get; private set; }
    public string UserId { get; private set; }
    public ReturnRequestStatus Status { get; private set; }
    public string Reason { get; private set; }
    public string? ResolutionNote { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public string? ReceivedByUserId { get; private set; }
    public string? ReceiveNote { get; private set; }

    private readonly List<ReturnRequestLine> _lines = [];
    public IReadOnlyCollection<ReturnRequestLine> Lines => _lines;

    private ReturnRequest()
    {
        UserId = string.Empty;
        Reason = string.Empty;
        Status = ReturnRequestStatus.Pending;
    }

    public static ReturnRequest Create(Guid purchaseOrderId, string userId, string reason)
    {
        BusinessException.ThrowIfTrue(
            purchaseOrderId == Guid.Empty,
            ErrorMessages.ReturnRequest.PurchaseOrderRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            userId,
            ErrorMessages.ReturnRequest.UserRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            reason,
            ErrorMessages.ReturnRequest.ReasonRequired,
            ErrorMessages.Exception.CommerceTitle);

        ReturnRequest returnRequest = new()
        {
            PurchaseOrderId = purchaseOrderId,
            UserId = userId,
            Reason = reason.Trim(),
            Status = ReturnRequestStatus.Pending
        };

        returnRequest.AddDomainEvent(new ReturnRequestCreatedDomainEvent(returnRequest.Id, purchaseOrderId, userId));
        return returnRequest;
    }

    public ReturnRequestLine AddLine(Guid purchaseOrderLineId, int quantity, string? reason = null)
    {
        BusinessException.ThrowIfTrue(
            Status is not ReturnRequestStatus.Pending,
            ErrorMessages.ReturnRequest.CannotMutateSubmittedRequest,
            ErrorMessages.Exception.CommerceTitle);

        ReturnRequestLine line = ReturnRequestLine.Create(Id, purchaseOrderLineId, quantity, reason);
        _lines.Add(line);

        return line;
    }

    public void Approve(string? resolutionNote = null)
    {
        BusinessException.ThrowIfTrue(
            Status is not ReturnRequestStatus.Pending,
            ErrorMessages.ReturnRequest.StatusMustBePending,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            _lines.Count is 0,
            ErrorMessages.ReturnRequest.LinesRequired,
            ErrorMessages.Exception.CommerceTitle);

        Status = ReturnRequestStatus.Approved;
        ResolutionNote = string.IsNullOrWhiteSpace(resolutionNote) ? null : resolutionNote.Trim();
    }

    public void Reject(string resolutionNote)
    {
        BusinessException.ThrowIfTrue(
            Status is not ReturnRequestStatus.Pending,
            ErrorMessages.ReturnRequest.StatusMustBePending,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            resolutionNote,
            ErrorMessages.ReturnRequest.ResolutionNoteRequired,
            ErrorMessages.Exception.CommerceTitle);

        Status = ReturnRequestStatus.Rejected;
        ResolutionNote = resolutionNote.Trim();
    }

    public void Receive(string receivedByUserId, string? receiveNote, IReadOnlyCollection<ReturnRequestLineReceiveInfo> lineReceipts)
    {
        BusinessException.ThrowIfTrue(
            Status is ReturnRequestStatus.Received,
            ErrorMessages.ReturnRequest.AlreadyReceived,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            Status is ReturnRequestStatus.Rejected,
            ErrorMessages.ReturnRequest.CannotReceiveRejected,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            Status is ReturnRequestStatus.Pending,
            ErrorMessages.ReturnRequest.CannotReceivePending,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            Status is not ReturnRequestStatus.Approved,
            ErrorMessages.ReturnRequest.NotApproved,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            receivedByUserId,
            ErrorMessages.ReturnRequest.UserRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            lineReceipts.Count is 0,
            ErrorMessages.ReturnRequest.LinesRequired,
            ErrorMessages.Exception.CommerceTitle);

        Dictionary<Guid, ReturnRequestLineReceiveInfo> receiptLookup = lineReceipts.ToDictionary(receipt => receipt.ReturnRequestLineId);
        foreach (ReturnRequestLine line in _lines)
        {
            receiptLookup.TryGetValue(line.Id, out ReturnRequestLineReceiveInfo? receipt);
            BusinessException.ThrowIfNull(
                receipt,
                ErrorMessages.ReturnRequest.LineNotFound,
                ErrorMessages.Exception.CommerceTitle);

            line.Receive(receipt!.ReceivedQuantity, receipt.RestockedQuantity, receipt.Note);
        }

        Status = ReturnRequestStatus.Received;
        ReceivedAt = DateTime.UtcNow;
        ReceivedByUserId = receivedByUserId;
        ReceiveNote = string.IsNullOrWhiteSpace(receiveNote) ? null : receiveNote.Trim();
    }

    public void Cancel()
    {
        BusinessException.ThrowIfTrue(
            Status is not ReturnRequestStatus.Pending,
            ErrorMessages.ReturnRequest.StatusMustBePending,
            ErrorMessages.Exception.CommerceTitle);

        Status = ReturnRequestStatus.Cancelled;
    }
}
