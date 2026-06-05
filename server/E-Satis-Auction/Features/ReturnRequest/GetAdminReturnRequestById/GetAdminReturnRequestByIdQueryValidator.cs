using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ReturnRequest.GetAdminReturnRequestById;

public sealed class GetAdminReturnRequestByIdQueryValidator : AbstractValidator<GetAdminReturnRequestByIdQuery>
{
    public GetAdminReturnRequestByIdQueryValidator()
    {
        RuleFor(query => query.ReturnRequestId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}
