using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.Auction.GetAuctions;

public sealed class GetAuctionsQueryValidator : PaginatedQueryValidator<GetAuctionsQuery>
{
    public GetAuctionsQueryValidator()
    {
        RuleFor(query => query.ProductId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.ProductId.HasValue);
    }
}
