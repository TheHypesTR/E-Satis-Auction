using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.ProductListing.GetAdminProductListings;

public sealed class GetAdminProductListingsQueryValidator : PaginatedQueryValidator<GetAdminProductListingsQuery>
{
    public GetAdminProductListingsQueryValidator()
    {
        RuleFor(query => query.Status)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.Status.HasValue);

        RuleFor(query => query.ProductId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.ProductId.HasValue);

        RuleFor(query => query.SourceFacilityId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.SourceFacilityId.HasValue);

        RuleFor(query => query.CategoryId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.CategoryId.HasValue);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(128).WithMessage(ErrorMessages.Validation.SearchTermLength);

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

        RuleFor(query => query)
            .Must(query => !query.StartDate.HasValue || !query.EndDate.HasValue || query.StartDate.Value <= query.EndDate.Value)
            .WithMessage(ErrorMessages.Validation.InvalidDateRange)
            .WithName("DateRange");
    }
}
