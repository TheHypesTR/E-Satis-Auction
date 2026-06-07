using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ReturnRequest.RejectReturnRequest;

public sealed class RejectReturnRequestCommandValidator : AbstractValidator<RejectReturnRequestCommand>
{
    public RejectReturnRequestCommandValidator()
    {
        RuleFor(command => command.ReturnRequestId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(command => command.Payload)
            .NotNull().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .ChildRules(payload =>
            {
                payload.RuleFor(x => x.Reason)
                    .NotEmpty().WithMessage(ErrorMessages.ReturnRequest.ResolutionNoteRequired)
                    .MaximumLength(1024).WithMessage(ErrorMessages.ReturnRequest.ResolutionNoteMaxLength);
            });
    }
}
