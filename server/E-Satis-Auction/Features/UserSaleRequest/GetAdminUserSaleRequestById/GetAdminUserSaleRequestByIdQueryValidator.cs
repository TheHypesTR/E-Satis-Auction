using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.UserSaleRequest.GetAdminUserSaleRequestById;

public sealed class GetAdminUserSaleRequestByIdQueryValidator : AbstractValidator<GetAdminUserSaleRequestByIdQuery>
{
    public GetAdminUserSaleRequestByIdQueryValidator()
    {
        RuleFor(query => query.RequestId).NotEmpty().WithMessage(ErrorMessages.UserSaleRequest.EntityName);
    }
}
