using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.PurchaseOrder.ApprovePurchaseOrder;

public sealed class ApprovePurchaseOrderCommandValidator : AbstractValidator<ApprovePurchaseOrderCommand>
{
    public ApprovePurchaseOrderCommandValidator()
    {
        RuleFor(command => command.PurchaseOrderId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(command => command.Payload)
            .NotNull().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .ChildRules(payload =>
            {
                payload.RuleFor(x => x.Note)
                    .MaximumLength(1024).WithMessage(ErrorMessages.PurchaseOrder.NoteMaxLength)
                    .When(x => !string.IsNullOrWhiteSpace(x.Note));
            });
    }
}
