using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Cart.UpdateCartListing;

public sealed class UpdateCartListingCommandValidator : AbstractValidator<UpdateCartListingCommand>
{
    public UpdateCartListingCommandValidator()
    {
        RuleFor(command => command.Payload.ProductListingId)
            .NotEmpty().WithMessage(ErrorMessages.PurchaseOrder.ProductListingRequired);

        RuleFor(command => command.Payload.Quantity)
            .GreaterThan(0).WithMessage(ErrorMessages.PurchaseOrder.QuantityMustBePositive);
    }
}
