using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ProductListing.DeleteProductListing;

public sealed class DeleteProductListingCommandValidator : AbstractValidator<DeleteProductListingCommand>
{
    public DeleteProductListingCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}
