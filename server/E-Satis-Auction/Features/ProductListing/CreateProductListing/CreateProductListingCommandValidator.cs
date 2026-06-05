using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ProductListing.CreateProductListing;

public sealed class CreateProductListingCommandValidator : AbstractValidator<CreateProductListingCommand>
{
    public CreateProductListingCommandValidator()
    {
        RuleFor(command => command.ProductId)
            .NotEmpty().WithMessage(ErrorMessages.ProductListing.ProductRequired);

        RuleFor(command => command.SourceFacilityId)
            .NotEmpty().WithMessage(ErrorMessages.ProductListing.SourceFacilityRequired);

        RuleFor(command => command.Price)
            .GreaterThan(0).WithMessage(ErrorMessages.ProductListing.PriceMustBePositive);

        RuleFor(command => command.Currency)
            .NotEmpty().WithMessage(ErrorMessages.ProductListing.CurrencyRequired)
            .Length(3).WithMessage(ErrorMessages.ProductListing.InvalidCurrency);

        RuleFor(command => command)
            .Must(command => !command.ActiveFrom.HasValue || !command.ActiveUntil.HasValue || command.ActiveFrom.Value <= command.ActiveUntil.Value)
            .WithMessage(ErrorMessages.ProductListing.InvalidActiveDateRange)
            .WithName("ActiveDateRange");
    }
}
