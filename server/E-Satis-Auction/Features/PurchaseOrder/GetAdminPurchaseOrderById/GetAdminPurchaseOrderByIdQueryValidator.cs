using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.PurchaseOrder.GetAdminPurchaseOrderById;

public sealed class GetAdminPurchaseOrderByIdQueryValidator : AbstractValidator<GetAdminPurchaseOrderByIdQuery>
{
    public GetAdminPurchaseOrderByIdQueryValidator()
    {
        RuleFor(query => query.PurchaseOrderId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}
