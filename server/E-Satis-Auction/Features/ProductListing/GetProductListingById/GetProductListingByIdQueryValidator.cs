using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ProductListing.GetProductListingById;

public sealed class GetProductListingByIdQueryValidator : AbstractValidator<GetProductListingByIdQuery>
{
    public GetProductListingByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}
