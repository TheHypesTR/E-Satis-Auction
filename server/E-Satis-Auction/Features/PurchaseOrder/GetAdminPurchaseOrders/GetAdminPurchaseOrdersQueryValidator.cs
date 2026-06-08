using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.PurchaseOrder.GetAdminPurchaseOrders;

public sealed class GetAdminPurchaseOrdersQueryValidator : PaginatedQueryValidator<GetAdminPurchaseOrdersQuery>
{
    public GetAdminPurchaseOrdersQueryValidator()
    {
        RuleFor(query => query.Status)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.Status.HasValue);

        RuleFor(query => query.OrderSource)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.OrderSource.HasValue);

        RuleFor(query => query.ProductListingId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.ProductListingId.HasValue);

        RuleFor(query => query.ProductId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.ProductId.HasValue);

        RuleFor(query => query)
            .Must(query => !query.StartDate.HasValue || !query.EndDate.HasValue || query.StartDate.Value <= query.EndDate.Value)
            .WithMessage(ErrorMessages.Validation.InvalidDateRange)
            .WithName("DateRange");
    }
}
