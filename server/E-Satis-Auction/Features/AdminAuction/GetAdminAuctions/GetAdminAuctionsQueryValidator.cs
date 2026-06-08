using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.AdminAuction.GetAdminAuctions;

public sealed class GetAdminAuctionsQueryValidator : PaginatedQueryValidator<GetAdminAuctionsQuery>
{
    public GetAdminAuctionsQueryValidator()
    {
        RuleFor(query => query.ProductListingId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.ProductListingId.HasValue);

        RuleFor(query => query.ProductId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.ProductId.HasValue);
    }
}
