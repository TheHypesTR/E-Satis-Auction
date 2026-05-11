using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Dispatch.CreateDispatch;

public sealed class CreateDispatchCommandValidator : AbstractValidator<CreateDispatchCommand>
{
    private const string TARGET_DESTINATION_ERROR_KEY = "TargetDestination";
    
    public CreateDispatchCommandValidator()
    {
        RuleFor(x => x.SourceFacilityId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        
        RuleFor(x => x.Payload)
            .NotNull().WithMessage(ErrorMessages.Validation.InvalidIdentifier) 
            .ChildRules(payload => 
            {
                payload.RuleFor(p => p)
                    .Must(p => p.TargetFacilityId.HasValue ^ p.TargetAddressId.HasValue)
                    .WithMessage(ErrorMessages.Dispatch.ExclusiveTargetRequired)
                    .WithName(TARGET_DESTINATION_ERROR_KEY);

                payload.RuleFor(p => p.ReceiverName)
                    .NotEmpty().WithMessage(ErrorMessages.Dispatch.ReceiverNameRequired)
                    .MaximumLength(128).WithMessage(ErrorMessages.Dispatch.ReceiverNameMaxLength);

                payload.RuleFor(p => p.ReceiverPhone)
                    .NotEmpty().WithMessage(ErrorMessages.Dispatch.ReceiverPhoneRequired)
                    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage(ErrorMessages.Validation.InvalidPhone);

                payload.RuleFor(p => p.Notes)
                    .MaximumLength(1024).WithMessage(ErrorMessages.Dispatch.NotesMaxLength)
                    .When(p => !string.IsNullOrWhiteSpace(p.Notes));

                payload.RuleFor(p => p.Items)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage(ErrorMessages.Dispatch.ItemsRequired)
                    .Must(items => items is { Count: > 0 }).WithMessage(ErrorMessages.Dispatch.ItemsRequired)
                    .Must(items => items.Select(i => i.ItemId).Distinct().Count() == items.Count)
                    .WithMessage(ErrorMessages.Dispatch.DuplicateItemsNotAllowed);

                payload.RuleForEach(p => p.Items).ChildRules(item =>
                {
                    item.RuleFor(i => i.ItemId)
                        .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage(ErrorMessages.Dispatch.QuantityMustBePositive);
                });
            });
    }
}