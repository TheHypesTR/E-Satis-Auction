using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.PartSaleOperation.CreatePartSaleOperation;

public sealed class CreatePartSaleOperationCommandValidator : AbstractValidator<CreatePartSaleOperationCommand>
{
    public CreatePartSaleOperationCommandValidator()
    {
        RuleFor(command => command.Payload.SourceItemId).NotEmpty().WithMessage(ErrorMessages.Dispatch.SourceItemRequired);
        RuleFor(command => command.Payload.ProductId).NotEmpty().WithMessage(ErrorMessages.Product.EntityName);
        RuleFor(command => command.Payload.FacilityId).NotEmpty().WithMessage(ErrorMessages.Item.FacilityRequired);
        RuleFor(command => command.Payload.Quantity).GreaterThan(0).WithMessage(ErrorMessages.PurchaseOrder.QuantityMustBePositive);
        RuleFor(command => command.Payload.UnitOfMeasure).IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        RuleFor(command => command.Payload.Notes).MaximumLength(1024);
    }
}
