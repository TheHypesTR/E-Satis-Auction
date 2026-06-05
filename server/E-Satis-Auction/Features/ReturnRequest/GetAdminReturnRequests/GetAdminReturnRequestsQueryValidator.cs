using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.ReturnRequest.GetAdminReturnRequests;

public sealed class GetAdminReturnRequestsQueryValidator : PaginatedQueryValidator<GetAdminReturnRequestsQuery>
{
    public GetAdminReturnRequestsQueryValidator()
    {
        RuleFor(query => query.Status)
            .IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.Status.HasValue);

        RuleFor(query => query.PurchaseOrderId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier)
            .When(query => query.PurchaseOrderId.HasValue);

        RuleFor(query => query)
            .Must(query => !query.StartDate.HasValue || !query.EndDate.HasValue || query.StartDate.Value <= query.EndDate.Value)
            .WithMessage(ErrorMessages.Validation.InvalidDateRange)
            .WithName("DateRange");
    }
}
