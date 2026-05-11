using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Interfaces.Messaging;
using FluentValidation;

namespace E_Satis_Auction.Common.Validators;

public abstract class PaginatedQueryValidator<T> : AbstractValidator<T> where T : IPaginatedQuery
{
    protected PaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage(ErrorMessages.Validation.InvalidPageNumber);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage(ErrorMessages.Validation.InvalidPageSize)
            .LessThanOrEqualTo(100).WithMessage(ErrorMessages.Validation.PageSizeExceeded);
    }
}