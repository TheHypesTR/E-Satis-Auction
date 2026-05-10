using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Enums;
using FluentValidation;

namespace e_Sat_Auction.Features.Dispatch.ReceiveDispatch;

public sealed class ReceiveDispatchCommandValidator : AbstractValidator<ReceiveDispatchCommand>
{
    public ReceiveDispatchCommandValidator()
    {
        RuleFor(x => x.DispatchId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.Payload)
            .NotNull().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .ChildRules(payload =>
            {
                payload.RuleFor(p => p.Items)
                    .Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage(ErrorMessages.Dispatch.ItemsRequired)
                    .Must(items => items is { Count: > 0 }).WithMessage(ErrorMessages.Dispatch.ItemsRequired)
                    .Must(items => items.Select(i => i.SourceItemId).Distinct().Count() == items.Count)
                    .WithMessage(ErrorMessages.Dispatch.DuplicateItemsNotAllowed);

                payload.RuleForEach(p => p.Items).ChildRules(item =>
                {
                    item.RuleFor(i => i.SourceItemId)
                        .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

                    item.RuleFor(i => i.Mode)
                        .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

                    item.RuleFor(i => i.ReceivedQuantity)
                        .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.Dispatch.ReceiptQuantityInvalid);

                    item.RuleFor(i => i.DamagedQuantity)
                        .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.Dispatch.ReceiptQuantityInvalid);

                    item.RuleFor(i => i)
                        .Must(i => i.ReceivedQuantity + i.DamagedQuantity > 0)
                        .WithMessage(ErrorMessages.Dispatch.ReceiptQuantityInvalid);

                    item.When(i => i.Mode == ItemMode.Standardized, () =>
                    {
                        item.RuleFor(i => i.MappedProductId)
                            .NotEmpty().WithMessage(ErrorMessages.Item.ProductIdRequiredForStandardized);
                    });

                    item.When(i => i.Mode == ItemMode.AdHoc, () =>
                    {
                        item.RuleFor(i => i.MappedProductId)
                            .Null().WithMessage(ErrorMessages.Item.ProductIdMustBeNullForAdHoc);
                    });
                });
                
                payload.RuleFor(p => p.DeliveryNote)
                    .MaximumLength(1024).WithMessage(ErrorMessages.Dispatch.DeliveryNoteMaxLength)
                    .When(p => !string.IsNullOrWhiteSpace(p.DeliveryNote));
            });
    }
}