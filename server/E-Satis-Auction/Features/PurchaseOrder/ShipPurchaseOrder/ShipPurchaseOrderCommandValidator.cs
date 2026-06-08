using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.PurchaseOrder.ShipPurchaseOrder;

public sealed class ShipPurchaseOrderCommandValidator : AbstractValidator<ShipPurchaseOrderCommand>
{
    public ShipPurchaseOrderCommandValidator()
    {
        RuleFor(command => command.PurchaseOrderId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(command => command.Payload)
            .NotNull().WithMessage(ErrorMessages.PurchaseOrder.ShippingInfoRequired)
            .ChildRules(payload =>
            {
                payload.RuleFor(x => x.CarrierName)
                    .NotEmpty().WithMessage(ErrorMessages.PurchaseOrder.CarrierNameRequired)
                    .MaximumLength(128).WithMessage(ErrorMessages.PurchaseOrder.CarrierNameMaxLength);

                payload.RuleFor(x => x.TrackingNumber)
                    .NotEmpty().WithMessage(ErrorMessages.PurchaseOrder.TrackingNumberRequired)
                    .MaximumLength(128).WithMessage(ErrorMessages.PurchaseOrder.TrackingNumberMaxLength);

                payload.RuleFor(x => x.TrackingUrl)
                    .MaximumLength(512).WithMessage(ErrorMessages.PurchaseOrder.TrackingUrlMaxLength)
                    .When(x => !string.IsNullOrWhiteSpace(x.TrackingUrl));

                payload.RuleFor(x => x.Notes)
                    .MaximumLength(1024).WithMessage(ErrorMessages.PurchaseOrder.NoteMaxLength)
                    .When(x => !string.IsNullOrWhiteSpace(x.Notes));
            });
    }
}
