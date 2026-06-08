using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.ReturnRequest.GetMyReturnRequestById;

public sealed class GetMyReturnRequestByIdQueryValidator : AbstractValidator<GetMyReturnRequestByIdQuery>
{
    public GetMyReturnRequestByIdQueryValidator()
    {
        RuleFor(query => query.ReturnRequestId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}
