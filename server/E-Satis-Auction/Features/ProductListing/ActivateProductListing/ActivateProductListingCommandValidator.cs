using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ProductListing.ActivateProductListing;

public sealed class ActivateProductListingCommandValidator : AbstractValidator<ActivateProductListingCommand>
{
    public ActivateProductListingCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}
