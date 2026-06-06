using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.UserSaleRequest.GetAdminUserSaleRequests;

public sealed class GetAdminUserSaleRequestsQueryValidator : PaginatedQueryValidator<GetAdminUserSaleRequestsQuery>
{
    public GetAdminUserSaleRequestsQueryValidator()
    {
        RuleFor(query => query.Status).IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier).When(query => query.Status.HasValue);
    }
}
