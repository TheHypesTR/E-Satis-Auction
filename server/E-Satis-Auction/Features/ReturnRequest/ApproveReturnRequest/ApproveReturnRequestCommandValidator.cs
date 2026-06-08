using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ReturnRequest.ApproveReturnRequest;

public sealed class ApproveReturnRequestCommandValidator : AbstractValidator<ApproveReturnRequestCommand>
{
    public ApproveReturnRequestCommandValidator()
    {
        RuleFor(command => command.ReturnRequestId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(command => command.Payload)
            .NotNull().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .ChildRules(payload =>
            {
                payload.RuleFor(x => x.Note)
                    .MaximumLength(1024).WithMessage(ErrorMessages.ReturnRequest.ResolutionNoteMaxLength)
                    .When(x => !string.IsNullOrWhiteSpace(x.Note));
            });
    }
}
