using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Dispatch.CancelDispatch;

public sealed class CancelDispatchCommandValidator : AbstractValidator<CancelDispatchCommand>
{
    public CancelDispatchCommandValidator()
    {
        RuleFor(x => x.DispatchId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        
        RuleFor(x => x.Payload)
            .NotNull().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .ChildRules(payload =>
            {
                payload.RuleFor(p => p.CancellationNote)
                    .MaximumLength(1024).WithMessage(ErrorMessages.Dispatch.DeliveryNoteMaxLength)
                    .When(p => !string.IsNullOrWhiteSpace(p.CancellationNote));
            });
    }
}