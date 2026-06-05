using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.PurchaseOrder.RejectPurchaseOrder;

public sealed class RejectPurchaseOrderCommandValidator : AbstractValidator<RejectPurchaseOrderCommand>
{
    public RejectPurchaseOrderCommandValidator()
    {
        RuleFor(command => command.PurchaseOrderId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(command => command.Payload)
            .NotNull().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .ChildRules(payload =>
            {
                payload.RuleFor(x => x.Reason)
                    .NotEmpty().WithMessage(ErrorMessages.PurchaseOrder.RejectionReasonRequired)
                    .MaximumLength(1024).WithMessage(ErrorMessages.PurchaseOrder.NoteMaxLength);
            });
    }
}
