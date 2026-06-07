using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ProductListing.DeactivateProductListing;

public sealed class DeactivateProductListingCommandValidator : AbstractValidator<DeactivateProductListingCommand>
{
    public DeactivateProductListingCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}
