using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Dispatch.CompleteAddressDispatch;

public sealed class CompleteAddressDispatchCommandValidator : AbstractValidator<CompleteAddressDispatchCommand>
{
    public CompleteAddressDispatchCommandValidator()
    {
        RuleFor(x => x.DispatchId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.Payload)
            .NotNull().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .ChildRules(payload =>
            {
                payload.RuleFor(p => p.DeliveryNote)
                    .MaximumLength(1024).WithMessage(ErrorMessages.Dispatch.DeliveryNoteMaxLength)
                    .When(p => !string.IsNullOrWhiteSpace(p.DeliveryNote));
            });
    }
}