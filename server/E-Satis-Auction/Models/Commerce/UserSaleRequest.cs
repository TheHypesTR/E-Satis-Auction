using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class UserSaleRequest : BaseEntity
{
    public string UserId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public decimal UserEstimatedValue { get; private set; }
    public decimal? AcquisitionPrice { get; private set; }
    public decimal? TargetResalePrice { get; private set; }
    public decimal? ExpectedProfit { get; private set; }
    public UserSaleRequestStatus Status { get; private set; }
    public string? AdminNote { get; private set; }
    public uint Version { get; private set; }

    private UserSaleRequest()
    {
        UserId = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
        Status = UserSaleRequestStatus.Pending;
    }

    public static UserSaleRequest Create(string userId, string title, string description, Guid categoryId, decimal userEstimatedValue)
    {
        ValidateUserPayload(userId, title, description, categoryId, userEstimatedValue);

        return new UserSaleRequest
        {
            UserId = userId,
            Title = title.Trim(),
            Description = description.Trim(),
            CategoryId = categoryId,
            UserEstimatedValue = userEstimatedValue,
            Status = UserSaleRequestStatus.Pending
        };
    }

    public void Approve(decimal acquisitionPrice, decimal targetResalePrice, string? adminNote)
    {
        EnsurePending();
        BusinessException.ThrowIfTrue(acquisitionPrice < 0 || targetResalePrice < 0, ErrorMessages.UserSaleRequest.AmountInvalid, ErrorMessages.Exception.CommerceTitle);

        AcquisitionPrice = acquisitionPrice;
        TargetResalePrice = targetResalePrice;
        ExpectedProfit = targetResalePrice - acquisitionPrice;
        AdminNote = string.IsNullOrWhiteSpace(adminNote) ? null : adminNote.Trim();
        Status = UserSaleRequestStatus.Approved;
    }

    public void Reject(string reason)
    {
        EnsurePending();
        BusinessException.ThrowIfNullOrWhiteSpace(reason, ErrorMessages.UserSaleRequest.RejectionReasonRequired, ErrorMessages.Exception.CommerceTitle);

        AdminNote = reason.Trim();
        Status = UserSaleRequestStatus.Rejected;
    }

    public void MarkIntakeCreated(string? adminNote)
    {
        BusinessException.ThrowIfTrue(Status is not UserSaleRequestStatus.Approved, ErrorMessages.UserSaleRequest.StatusMustBeApproved, ErrorMessages.Exception.CommerceTitle);
        AdminNote = string.IsNullOrWhiteSpace(adminNote) ? AdminNote : adminNote.Trim();
        Status = UserSaleRequestStatus.IntakeCreated;
    }

    private void EnsurePending()
    {
        BusinessException.ThrowIfTrue(Status is not UserSaleRequestStatus.Pending, ErrorMessages.UserSaleRequest.StatusMustBePending, ErrorMessages.Exception.CommerceTitle);
    }

    private static void ValidateUserPayload(string userId, string title, string description, Guid categoryId, decimal userEstimatedValue)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(userId, ErrorMessages.PurchaseOrder.UserRequired, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfNullOrWhiteSpace(title, ErrorMessages.UserSaleRequest.TitleRequired, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfNullOrWhiteSpace(description, ErrorMessages.UserSaleRequest.DescriptionRequired, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(categoryId == Guid.Empty, ErrorMessages.Category.EntityName, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(userEstimatedValue < 0, ErrorMessages.UserSaleRequest.AmountInvalid, ErrorMessages.Exception.CommerceTitle);
    }
}
