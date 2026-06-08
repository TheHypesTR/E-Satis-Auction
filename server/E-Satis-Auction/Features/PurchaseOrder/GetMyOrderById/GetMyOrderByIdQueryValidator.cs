using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.PurchaseOrder.GetMyOrderById;

public sealed class GetMyOrderByIdQueryValidator : AbstractValidator<GetMyOrderByIdQuery>
{
    public GetMyOrderByIdQueryValidator()
    {
        RuleFor(query => query.PurchaseOrderId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}
