using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.ProductListing.GetProductListings;

public sealed class GetProductListingsQueryValidator : PaginatedQueryValidator<GetProductListingsQuery>
{
    public GetProductListingsQueryValidator()
    {
        RuleFor(query => query.SearchTerm)
            .MaximumLength(128).WithMessage(ErrorMessages.Validation.SearchTermLength);

        RuleFor(query => query.ProductId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.ProductId.HasValue);

        RuleFor(query => query.CategoryId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.CategoryId.HasValue);

        RuleFor(query => query.SourceFacilityId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.SourceFacilityId.HasValue);

        RuleFor(query => query.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.ProductListing.PriceMustBePositive)
            .When(query => query.MinPrice.HasValue);

        RuleFor(query => query.MaxPrice)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.ProductListing.PriceMustBePositive)
            .When(query => query.MaxPrice.HasValue);

        RuleFor(query => query)
            .Must(query => !query.MinPrice.HasValue || !query.MaxPrice.HasValue || query.MinPrice.Value <= query.MaxPrice.Value)
            .WithMessage(ErrorMessages.Validation.InvalidDateRange)
            .WithName("PriceRange");
    }
}
