using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.InventoryTransaction.GetAllInventoryTransactions;

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