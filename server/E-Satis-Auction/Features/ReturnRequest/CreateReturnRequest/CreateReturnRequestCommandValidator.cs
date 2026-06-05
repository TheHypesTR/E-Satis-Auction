using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ReturnRequest.CreateReturnRequest;

public sealed class CreateReturnRequestCommandValidator : AbstractValidator<CreateReturnRequestCommand>
{
    public CreateReturnRequestCommandValidator()
    {
        RuleFor(command => command.PurchaseOrderId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(command => command.Payload)
            .NotNull().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .ChildRules(payload =>
            {
                payload.RuleFor(x => x.Reason)
                    .NotEmpty().WithMessage(ErrorMessages.ReturnRequest.ReasonRequired)
                    .MaximumLength(1024).WithMessage(ErrorMessages.ReturnRequest.ReasonMaxLength);

                payload.RuleFor(x => x.Lines)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage(ErrorMessages.ReturnRequest.LinesRequired)
                    .Must(lines => lines is { Count: > 0 }).WithMessage(ErrorMessages.ReturnRequest.LinesRequired)
                    .Must(lines => lines.Select(line => line.PurchaseOrderLineId).Distinct().Count() == lines.Count)
                    .WithMessage(ErrorMessages.ReturnRequest.DuplicateLinesNotAllowed);

                payload.RuleForEach(x => x.Lines).ChildRules(line =>
                {
                    line.RuleFor(x => x.PurchaseOrderLineId)
                        .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

                    line.RuleFor(x => x.Quantity)
                        .GreaterThan(0).WithMessage(ErrorMessages.ReturnRequest.QuantityMustBePositive);

                    line.RuleFor(x => x.Reason)
                        .MaximumLength(1024).WithMessage(ErrorMessages.ReturnRequest.ReasonMaxLength)
                        .When(x => !string.IsNullOrWhiteSpace(x.Reason));
                });
            });
    }
}
