using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.UserSaleRequest.GetMyUserSaleRequestById;

public sealed class GetMyUserSaleRequestByIdQueryValidator : AbstractValidator<GetMyUserSaleRequestByIdQuery>
{
    public GetMyUserSaleRequestByIdQueryValidator()
    {
        RuleFor(query => query.RequestId).NotEmpty().WithMessage(ErrorMessages.UserSaleRequest.EntityName);
    }
}
