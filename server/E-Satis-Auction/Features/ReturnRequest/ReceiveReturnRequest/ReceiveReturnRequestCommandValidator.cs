using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ReturnRequest.ReceiveReturnRequest;

public sealed class ReceiveReturnRequestCommandValidator : AbstractValidator<ReceiveReturnRequestCommand>
{
    public ReceiveReturnRequestCommandValidator()
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

                payload.RuleFor(x => x.TargetFacilityId)
                    .NotEqual(Guid.Empty).WithMessage(ErrorMessages.Validation.InvalidIdentifier)
                    .When(x => x.TargetFacilityId.HasValue);

                payload.RuleFor(x => x.Lines)
                    .Must(lines => lines is null || lines.Select(line => line.ReturnRequestLineId).Distinct().Count() == lines.Count)
                    .WithMessage(ErrorMessages.ReturnRequest.DuplicateLinesNotAllowed);

                payload.RuleForEach(x => x.Lines).ChildRules(line =>
                {
                    line.RuleFor(x => x.ReturnRequestLineId)
                        .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

                    line.RuleFor(x => x.ReceivedQuantity)
                        .GreaterThan(0).WithMessage(ErrorMessages.ReturnRequest.InvalidReceiveQuantity);

                    line.RuleFor(x => x.RestockQuantity)
                        .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.ReturnRequest.InvalidRestockQuantity)
                        .LessThanOrEqualTo(x => x.ReceivedQuantity).WithMessage(ErrorMessages.ReturnRequest.InvalidRestockQuantity);

                    line.RuleFor(x => x.Note)
                        .MaximumLength(1024).WithMessage(ErrorMessages.ReturnRequest.ResolutionNoteMaxLength)
                        .When(x => !string.IsNullOrWhiteSpace(x.Note));
                });
            });
    }
}
