using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Validators;
using FluentValidation;

namespace e_Sat_Auction.Features.InventoryTransaction.GetAllInventoryTransactions;

public class GetAllInventoryTransactionsQueryValidator : PaginatedQueryValidator<GetAllInventoryTransactionsQuery>
{
    public GetAllInventoryTransactionsQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage(ErrorMessages.Validation.InvalidDateRange);
    }
}