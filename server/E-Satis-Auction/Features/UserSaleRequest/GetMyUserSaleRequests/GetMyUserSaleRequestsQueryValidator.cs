using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.UserSaleRequest.GetMyUserSaleRequests;

public sealed class GetMyUserSaleRequestsQueryValidator : PaginatedQueryValidator<GetMyUserSaleRequestsQuery>
{
    public GetMyUserSaleRequestsQueryValidator()
    {
        RuleFor(query => query.Status).IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier).When(query => query.Status.HasValue);
    }
}
