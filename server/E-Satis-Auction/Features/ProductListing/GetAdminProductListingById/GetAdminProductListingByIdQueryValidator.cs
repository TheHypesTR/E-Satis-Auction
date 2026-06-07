using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ProductListing.GetAdminProductListingById;

public sealed class GetAdminProductListingByIdQueryValidator : AbstractValidator<GetAdminProductListingByIdQuery>
{
    public GetAdminProductListingByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}
