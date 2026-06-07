using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.PurchaseOrder.GetMyOrders;

public sealed class GetMyOrdersQueryValidator : PaginatedQueryValidator<GetMyOrdersQuery>
{
    public GetMyOrdersQueryValidator()
    {
        RuleFor(query => query.Status)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.Status.HasValue);

        RuleFor(query => query.OrderSource)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.OrderSource.HasValue);

        RuleFor(query => query)
            .Must(query => !query.StartDate.HasValue || !query.EndDate.HasValue || query.StartDate.Value <= query.EndDate.Value)
            .WithMessage(ErrorMessages.Validation.InvalidDateRange)
            .WithName("DateRange");
    }
}
